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
using Umbraco.Core.Persistence;
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

            if (dataJson == null)
                return null;

            if (UmbracoContext.Current == null)
                return ConvertValue(id, contentTypeAlias, dataJson);

            return (IPublishedContent)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                $"Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.ConvertValueToContent_{id}_{contentTypeAlias}",
                () =>
                {
                    return ConvertValue(id, contentTypeAlias, dataJson);
                });
        }

        private static IPublishedContent ConvertValue(string id, string contentTypeAlias, string dataJson)
        {
            using (var timer = ApplicationContext.Current.ProfilingLogger.DebugDuration<DocTypeGridEditorHelper>(string.Format("ConvertValue ({0}, {1})", id, contentTypeAlias)))
            {
                var contentTypes = GetContentTypesByAlias(contentTypeAlias);
                var properties = new List<IPublishedProperty>();

                // Convert all the properties
                var data = JsonConvert.DeserializeObject(dataJson);
                var propValues = ((JObject)data).ToObject<Dictionary<string, object>>();
                foreach (var jProp in propValues)
                {
                    var propType = contentTypes.PublishedContentType.GetPropertyType(jProp.Key);
                    if (propType != null)
                    {
                        /* Because we never store the value in the database, we never run the property editors
                         * "ConvertEditorToDb" method however the property editors will expect their value to 
                         * be in a "DB" state so to get round this, we run the "ConvertEditorToDb" here before
                         * we go on to convert the value for the view. 
                         */
                        var propEditor = PropertyEditorResolver.Current.GetByAlias(propType.PropertyEditorAlias);
                        var propPreValues = GetPreValuesCollectionByDataTypeId(propType.DataTypeId);

                        var contentPropData = new ContentPropertyData(
                            jProp.Value,
                            propPreValues,
                            new Dictionary<string, object>());

                        var newValue = propEditor.ValueEditor.ConvertEditorToDb(contentPropData, jProp.Value);

                        /* Now that we have the DB stored value, we actually need to then convert it into it's
                         * XML serialized state as expected by the published property by calling ConvertDbToString
                         */
                        var propType2 = contentTypes.ContentType.CompositionPropertyTypes.Single(x => x.Alias.InvariantEquals(propType.PropertyTypeAlias));

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
                        catch (Exception ex)
                        {
                            LogHelper.Error<DocTypeGridEditorHelper>("[DocTypeGridEditor] Error creating Property object.", ex);
                        }

                        if (prop2 != null)
                        {
                            var newValue2 = propEditor.ValueEditor.ConvertDbToString(prop2, propType2, Services.DataTypeService);

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

                // Create the model based on our implementation of IPublishedContent
                IPublishedContent content = new DetachedPublishedContent(
                    nameObj != null ? nameObj.ToString() : null,
                    contentTypes.PublishedContentType,
                    properties.ToArray(),
                    containerNode);

                if (PublishedContentModelFactoryResolver.HasCurrent && PublishedContentModelFactoryResolver.Current.HasValue)
                {
                    // Let the current model factory create a typed model to wrap our model
                    content = PublishedContentModelFactoryResolver.Current.Factory.CreateModel(content);
                }

                return content;
            }

        }

        private static PreValueCollection GetPreValuesCollectionByDataTypeId(int dataTypeId)
        {
            return (PreValueCollection)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_", dataTypeId),
                () => Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId));
        }

        private static ContentTypeContainer GetContentTypesByAlias(string contentTypeAlias)
        {
            Guid contentTypeGuid;
            if (Guid.TryParse(contentTypeAlias, out contentTypeGuid))
                contentTypeAlias = GetContentTypeAliasByGuid(contentTypeGuid);

            return (ContentTypeContainer)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_", contentTypeAlias),
                () => new ContentTypeContainer
                {
                    PublishedContentType = PublishedContentType.Get(PublishedItemType.Content, contentTypeAlias),
                    ContentType = Services.ContentTypeService.GetContentType(contentTypeAlias)
                });
        }

        private static string GetContentTypeAliasByGuid(Guid contentTypeGuid)
        {
            return (string)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_", contentTypeGuid),
                () => Services.ContentTypeService.GetAliasByGuid(contentTypeGuid));
        }

        public static void RemapDocTypeAlias(string oldAlias, string newAlias, Transaction transaction = null)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            // Update references in property data
            // We do 2 very similar replace statements, but one is without spaces in the JSON, the other is with spaces 
            // as we can't guarantee what format it will actually get saved in
            var sql1 = string.Format(@"UPDATE cmsPropertyData
SET dataNtext = CAST(REPLACE(REPLACE(CAST(dataNtext AS nvarchar(max)), '""dtgeContentTypeAlias"":""{0}""', '""dtgeContentTypeAlias"":""{1}""'), '""dtgeContentTypeAlias"": ""{0}""', '""dtgeContentTypeAlias"": ""{1}""') AS ntext)
WHERE dataNtext LIKE '%""dtgeContentTypeAlias"":""{0}""%' OR dataNtext LIKE '%""dtgeContentTypeAlias"": ""{0}""%'", oldAlias, newAlias);

            if (transaction == null)
            {
                using (var tr = db.GetTransaction())
                {
                    db.Execute(sql1);
                    tr.Complete();
                }
            }
            else
            {
                db.Execute(sql1);
            }
        }

        public static void RemapPropertyAlias(string docTypeAlias, string oldAlias, string newAlias, Transaction transaction = null)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            // Update references in property data
            // We have to do it in code because there could be nested JSON so 
            // we need to make sure it only replaces at the specific level only
            Action doQuery = () =>
            {
                var rows = GetPropertyDataRows(docTypeAlias);
                foreach (var row in rows)
                {
                    var tokens = row.Data.SelectTokens(string.Format("$..controls[?(@.value.dtgeContentTypeAlias == '{0}' && @.value.value.{1})].value", docTypeAlias, oldAlias)).ToList();
                    if (tokens.Any())
                    {
                        foreach (var token in tokens)
                        {
                            token["value"][oldAlias].Rename(newAlias);
                        }
                        db.Execute("UPDATE [cmsPropertyData] SET [dataNtext] = @0 WHERE [id] = @1", row.RawData, row.Id);
                    }
                }
            };

            if (transaction == null)
            {
                using (var tr = db.GetTransaction())
                {
                    doQuery();
                    tr.Complete();
                }
            }
            else
            {
                doQuery();
            }
        }

        private static IEnumerable<JsonDbRow> GetPropertyDataRows(string docTypeAlias)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            return db.Query<JsonDbRow>(string.Format(
                @"SELECT [id], [dataNtext] as [rawdata] FROM cmsPropertyData WHERE dataNtext LIKE '%""dtgeContentTypeAlias"":""{0}""%' OR dataNtext LIKE '%""dtgeContentTypeAlias"": ""{0}""%'",
                docTypeAlias)).ToList();
        }
    }

    public class ContentTypeContainer
    {
        public PublishedContentType PublishedContentType { get; set; }
        public IContentType ContentType { get; set; }
    }
}