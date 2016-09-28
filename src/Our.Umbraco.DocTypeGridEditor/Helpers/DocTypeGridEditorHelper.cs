using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Our.Umbraco.DocTypeGridEditor.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
{
    public class DocTypeGridEditorHelper
    {
        private static ServiceContext Services
        {
            get { return ApplicationContext.Current.Services; }
        }

        public static IPublishedContent ConvertValueToContent(string id, string contentTypeAlias, string dataJson)
        {
            if (string.IsNullOrWhiteSpace(contentTypeAlias))
                return null;

            if (UmbracoContext.Current == null)
                return ConvertValue(id, contentTypeAlias, dataJson);

            return (IPublishedContent)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                string.Concat("DocTypeGridEditorHelper.ConvertValueToContent_", id, "_", contentTypeAlias),
                () =>
                {
                    return ConvertValue(id, contentTypeAlias, dataJson);
                });
        }

        private static IPublishedContent ConvertValue(string id, string contentTypeAlias, string dataJson)
        {
            using (var timer = DisposableTimer.DebugDuration<DocTypeGridEditorHelper>(string.Format("ConvertValueToContent ({0}, {1})", id, contentTypeAlias)))
            {
                Guid contentTypeGuid;
                if (Guid.TryParse(contentTypeAlias, out contentTypeGuid))
                    contentTypeAlias = Services.ContentTypeService.GetAliasByGuid(contentTypeGuid);

                var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, contentTypeAlias);
                var contentType = ApplicationContext.Current.Services.ContentTypeService.GetContentType(contentTypeAlias);
                var properties = new List<IPublishedProperty>();

                // Convert all the properties
                var data = JsonConvert.DeserializeObject(dataJson);
                var propValues = ((JObject)data).ToObject<Dictionary<string, object>>();
                foreach (var jProp in propValues)
                {
                    var propType = publishedContentType.GetPropertyType(jProp.Key);
                    if (propType != null)
                    {
                        /* Because we never store the value in the database, we never run the property editors
                         * "ConvertEditorToDb" method however the property editors will expect their value to 
                         * be in a "DB" state so to get round this, we run the "ConvertEditorToDb" here before
                         * we go on to convert the value for the view. 
                         */
                        var propEditor = PropertyEditorResolver.Current.GetByAlias(propType.PropertyEditorAlias);
                        var propPreValues = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(
                            propType.DataTypeId);

                        var contentPropData = new ContentPropertyData(
                            jProp.Value,
                            propPreValues,
                            new Dictionary<string, object>());

                        var newValue = propEditor.ValueEditor.ConvertEditorToDb(contentPropData, jProp.Value);

                        /* Now that we have the DB stored value, we actually need to then convert it into it's
                         * XML serialized state as expected by the published property by calling ConvertDbToString
                         */
                        var propType2 = contentType.CompositionPropertyTypes.Single(x => x.Alias.InvariantEquals(propType.PropertyTypeAlias));

                        Property prop2 = null;
                        try
                        {
                            /* HACK: [LK:2016-04-01] When using the "Umbraco.Tags" property-editor, the converted DB value does
                             * not match the datatypes underlying db-column type. So it throws a "Type validation failed" exception.
                             * We feel that the Umbraco core isn't handling the Tags value correctly, as it should be the responsiblity
                             * of the "Umbraco.Tags" property-editor to handle the value conversion into the correct type.
                             * See: http://issues.umbraco.org/issue/U4-8279
                             */
                            prop2 = new Property(propType2, newValue);
                        }
                        catch(Exception ex)
                        {
                            LogHelper.Error<DocTypeGridEditorHelper>("[DocTypeGridEditor] Error creating Property object.", ex);
                        }

                        if (prop2 != null)
                        {
                            var newValue2 = propEditor.ValueEditor.ConvertDbToString(prop2, propType2,
                                ApplicationContext.Current.Services.DataTypeService);

                            properties.Add(new DetachedPublishedProperty(propType, newValue2));
                        }
                    }
                }

                // Parse out the name manually
                object nameObj = null;
                if (propValues.TryGetValue("name", out nameObj))
                {
                    // Do nothing, we just want to parse out the name if we can
                }

                // Get the current request node we are embedded in
                var pcr = UmbracoContext.Current != null ? UmbracoContext.Current.PublishedContentRequest : null;
                var containerNode = pcr != null && pcr.HasPublishedContent ? pcr.PublishedContent : null;

                return new DetachedPublishedContent(nameObj != null ? nameObj.ToString() : null,
                    publishedContentType,
                    properties.ToArray(),
                    containerNode);
            }

        }
    }
}