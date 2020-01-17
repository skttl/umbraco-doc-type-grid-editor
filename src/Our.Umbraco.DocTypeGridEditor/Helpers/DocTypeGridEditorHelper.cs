using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Our.Umbraco.DocTypeGridEditor.Models;
using Our.Umbraco.DocTypeGridEditor.ValueProcessing;
using Our.Umbraco.DocTypeGridEditor.ValueProcessing.Collections;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
{
    public static class DocTypeGridEditorHelper
    {

        public static IPublishedElement ConvertValueToContent(string id, string contentTypeAlias, string dataJson)
        {
            if (string.IsNullOrWhiteSpace(contentTypeAlias))
                return null;

            if (dataJson == null)
                return null;

            if (Current.UmbracoContext == null)
                return ConvertValue(id, contentTypeAlias, dataJson);
            
            return (IPublishedElement)Current.AppCaches.RequestCache.GetCacheItem(
                $"Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.ConvertValueToContent_{id}_{contentTypeAlias}",
                () =>
                {
                    return ConvertValue(id, contentTypeAlias, dataJson);
                });
        }

        private static IPublishedElement ConvertValue(string id, string contentTypeAlias, string dataJson)
        {
            var contentTypes = GetContentTypesByAlias(contentTypeAlias);
            var properties = new List<IPublishedProperty>();

            // Convert all the properties
            var data = JsonConvert.DeserializeObject(dataJson);
            var propValues = ((JObject)data).ToObject<Dictionary<string, object>>();
            foreach (var jProp in propValues)
            {
                var propType = contentTypes.PublishedContentType.GetPropertyType(jProp.Key);
                if (propType == null)
                    continue;


                /* Because we never store the value in the database, we never run the property editors
                     * "ConvertEditorToDb" method however the property editors will expect their value to 
                     * be in a "DB" state so to get round this, we run the "ConvertEditorToDb" here before
                     * we go on to convert the value for the view. 
                     */
                Current.PropertyEditors.TryGet(propType.EditorAlias, out var propEditor);
                var propPreValues = GetPreValuesCollectionByDataTypeId(propType.DataType.Id);

                var contentPropData = new ContentPropertyData(jProp.Value, propPreValues);

                var newValue = propEditor.GetValueEditor().FromEditor(contentPropData, jProp.Value);

                //TODO: Add ValueProcessor here.
                var processorsCollection = Current.Factory.GetInstance<DocTypeGridEditorValueProcessorsCollection>();
                var processor = processorsCollection.FirstOrDefault(x => x.IsProcessorFor(propEditor.Alias));
                if (processor != null)
                {
                    newValue = processor.ProcessValue(newValue);
                }

                /* Now that we have the DB stored value, we actually need to then convert it into its
                 * XML serialized state as expected by the published property by calling ConvertDbToString
                 */
                var propType2 = contentTypes.ContentType.CompositionPropertyTypes.First(x => x.PropertyEditorAlias.InvariantEquals(propType.DataType.EditorAlias));

                Property prop2 = null;
                try
                {
                    /* HACK: [LK:2016-04-01] When using the "Umbraco.Tags" property-editor, the converted DB value does
                         * not match the datatypes underlying db-column type. So it throws a "Type validation failed" exception.
                         * We feel that the Umbraco core isn't handling the Tags value correctly, as it should be the responsiblity
                         * of the "Umbraco.Tags" property-editor to handle the value conversion into the correct type.
                         * See: http://issues.umbraco.org/issue/U4-8279
                         */
                    prop2 = new Property(propType2);
                    prop2.SetValue(newValue);
                }
                catch (Exception ex)
                {
                    Current.Logger.Error(typeof(DocTypeGridEditorHelper), ex, "[DocTypeGridEditor] Error creating Property object.");
                }

                if (prop2 != null)
                {
                    var newValue2 = propEditor.GetValueEditor().ConvertDbToString(propType2, newValue, Current.Services.DataTypeService);

                    properties.Add(new DetachedPublishedProperty(propType, newValue2));
                }
            }

            // Manually parse out the special properties
            propValues.TryGetValue("name", out object nameObj);
            Guid.TryParse(id, out Guid key);

            // Get the current request node we are embedded in

            var pcr = Current.UmbracoContext.PublishedRequest;
            var containerNode = pcr != null && pcr.HasPublishedContent ? pcr.PublishedContent : null;

            // Create the model based on our implementation of IPublishedElement
            IPublishedElement content = new DetachedPublishedElement(
                key,
                contentTypes.PublishedContentType,
                properties.ToArray());

            var publishedModelFactory = Current.Factory.GetInstance<IPublishedModelFactory>();

            if (publishedModelFactory != null)
            {
                // Let the current model factory create a typed model to wrap our model
                content = publishedModelFactory.CreateModel(content);
            }

            return content;

        }

        private static object GetPreValuesCollectionByDataTypeId(int dataTypeId)
        {
            return (object)Current.AppCaches.RuntimeCache.GetCacheItem(
                string.Concat(
                    "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_",
                    dataTypeId),
                () => Current.Services.DataTypeService.GetDataType(dataTypeId).Configuration);
        }

        private static ContentTypeContainer GetContentTypesByAlias(string contentTypeAlias)
        {
            if (Guid.TryParse(contentTypeAlias, out Guid contentTypeGuid))
                contentTypeAlias = GetContentTypeAliasByGuid(contentTypeGuid);

            return (ContentTypeContainer)Current.AppCaches.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_", contentTypeAlias),
                () => new ContentTypeContainer
                {
                    PublishedContentType = new PublishedContentType(Current.Services.ContentTypeService.Get(contentTypeAlias), Current.PublishedContentTypeFactory),
                    ContentType = Current.Services.ContentTypeService.Get(contentTypeAlias)
                });
        }

        private static string GetContentTypeAliasByGuid(Guid contentTypeGuid)
        {
            return (string)Current.AppCaches.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_", contentTypeGuid),
                () => Current.Services.ContentTypeService.GetAliasByGuid(contentTypeGuid));
        }

        public static void RemapDocTypeAlias(string oldAlias, string newAlias)
        {
            // Update references in property data
            // We do 2 very similar replace statements, but one is without spaces in the JSON, the other is with spaces 
            // as we can't guarantee what format it will actually get saved in
            var sql1 = string.Format(@"UPDATE cmsPropertyData
SET dataNtext = CAST(REPLACE(REPLACE(CAST(dataNtext AS nvarchar(max)), '""dtgeContentTypeAlias"":""{0}""', '""dtgeContentTypeAlias"":""{1}""'), '""dtgeContentTypeAlias"": ""{0}""', '""dtgeContentTypeAlias"": ""{1}""') AS ntext)
WHERE dataNtext LIKE '%""dtgeContentTypeAlias"":""{0}""%' OR dataNtext LIKE '%""dtgeContentTypeAlias"": ""{0}""%'", oldAlias, newAlias);

            using (var scope = Current.ScopeProvider.CreateScope())
                scope.Database.Execute(sql1);
        }

        public static void RemapPropertyAlias(string docTypeAlias, string oldAlias, string newAlias)
        {
            // Update references in property data
            // We have to do it in code because there could be nested JSON so 
            // we need to make sure it only replaces at the specific level only
            Action doQuery = () =>
            {
                var rows = GetPropertyDataRows(docTypeAlias);
                foreach (var row in rows)
                {
                    var tokens = row.Data.SelectTokens($"$..controls[?(@.value.dtgeContentTypeAlias == '{docTypeAlias}' && @.value.value.{oldAlias})].value").ToList();
                    if (tokens.Any() == false)
                        continue;

                    foreach (var token in tokens)
                    {
                        token["value"][oldAlias].Rename(newAlias);
                    }
                    using (var scope = Current.ScopeProvider.CreateScope())
                        scope.Database.Execute("UPDATE [cmsPropertyData] SET [dataNtext] = @0 WHERE [id] = @1", row.RawData, row.Id);
                }
            };

            doQuery();
        }

        private static IEnumerable<JsonDbRow> GetPropertyDataRows(string docTypeAlias)
        {
            using (var scope = Current.ScopeProvider.CreateScope())
                return scope.Database.Query<JsonDbRow>(
                    $@"SELECT [id], [dataNtext] as [rawdata] FROM cmsPropertyData WHERE dataNtext LIKE '%""dtgeContentTypeAlias"":""{docTypeAlias}""%' OR dataNtext LIKE '%""dtgeContentTypeAlias"": ""{docTypeAlias}""%'").ToList();
        }
    }

    public class ContentTypeContainer
    {
        public PublishedContentType PublishedContentType { get; set; }

        public IContentType ContentType { get; set; }
    }
}