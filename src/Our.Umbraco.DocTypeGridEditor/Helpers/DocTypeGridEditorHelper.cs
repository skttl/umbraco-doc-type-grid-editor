using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.DocTypeGridEditor.Models;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Umbraco.Core;
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

        public static IPublishedContent ConvertValueToContent(string id, string docTypeAlias, string dataJson)
        {
            if (string.IsNullOrWhiteSpace(docTypeAlias))
                return null;

           if (UmbracoContext.Current != null)
           {
            return (IPublishedContent)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                "DocTypeGridEditorHelper.ConvertValueToContent_" + id + "_" + docTypeAlias, () =>
                {
                   return ConvertValue(id, docTypeAlias, dataJson);
                 });
            }
                return (IPublishedContent) ConvertValue(id, docTypeAlias, dataJson);
        }

        private static IPublishedContent ConvertValue(string id, string docTypeAlias, string dataJson)
        {
                    using (var timer =  DisposableTimer.DebugDuration<DocTypeGridEditorHelper>(string.Format("ConvertValueToContent ({0}, {1})", id, docTypeAlias)))
                    {
                        Guid docTypeGuid;
                        if (Guid.TryParse(docTypeAlias, out docTypeGuid))
                            docTypeAlias = Services.ContentTypeService.GetAliasByGuid(docTypeGuid);

                        var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, docTypeAlias);
                        var contentType = ApplicationContext.Current.Services.ContentTypeService.GetContentType(docTypeAlias);
                        var properties = new List<IPublishedProperty>(); 

                        // Convert all the properties
                        var data = JsonConvert.DeserializeObject(dataJson);
                        var propValues = ((JObject) data).ToObject<Dictionary<string, object>>();
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
                                    jProp.Value == null ? null : jProp.Value.ToString(),
                                    propPreValues,
                                    new Dictionary<string, object>());

                                var newValue = propEditor.ValueEditor.ConvertEditorToDb(contentPropData, jProp.Value);

                                /* Now that we have the DB stored value, we actually need to then convert it into it's
                                 * XML serialized state as expected by the published property by calling ConvertDbToString
                                 */
                                var propType2 = contentType.CompositionPropertyTypes.Single(x => x.Alias == propType.PropertyTypeAlias);
                                var newValue2 = propEditor.ValueEditor.ConvertDbToString(new Property(propType2, newValue), propType2, 
                                    ApplicationContext.Current.Services.DataTypeService);

                                properties.Add(new DetachedPublishedProperty(propType, newValue2));
                            }
                        }

                        // Parse out the name manually
                        object nameObj = null;
                        if (propValues.TryGetValue("name", out nameObj))
                        {
                            // Do nothing, we just want to parse out the name if we can
                        }

                        // Get the current request node we are embedded in
                        var pcr = UmbracoContext.Current == null ? null : UmbracoContext.Current.PublishedContentRequest;
                        var containerNode = pcr != null && pcr.HasPublishedContent ? pcr.PublishedContent : null;

                        return new DetachedPublishedContent(nameObj == null ? null : nameObj.ToString(), 
                            publishedContentType,
                            properties.ToArray(),
                            containerNode);
                    }

        }
    }
}