using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Microsoft.Extensions.Logging;
using Umbraco.Extensions;
using Our.Umbraco.DocTypeGridEditor9.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.DocTypeGridEditor9.Helpers
{
    public class DocTypeGridEditorHelper
    {
        private readonly AppCaches _appCaches;
        private readonly IUmbracoContextAccessor _umbracoContext;
        private readonly PropertyEditorCollection _dataEditors;
        private readonly ILogger<DocTypeGridEditorHelper> _logger;
        private readonly IDataTypeService _dataTypeService;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IContentTypeService _contentTypeService;
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;

        public DocTypeGridEditorHelper(AppCaches appCaches, IUmbracoContextAccessor umbracoContextAccessor, PropertyEditorCollection dataEditors, ILoggerFactory loggerFactory, IDataTypeService dataTypeService, IPublishedModelFactory publishedModelFactory, IContentTypeService contentTypeService, IPublishedContentTypeFactory publishedContentTypeFactory)
        {
            _appCaches = appCaches;
            _umbracoContext = umbracoContextAccessor;
            _dataEditors = dataEditors;
            _logger = loggerFactory.CreateLogger<DocTypeGridEditorHelper>();
            _dataTypeService = dataTypeService;
            _publishedModelFactory = publishedModelFactory;
            _contentTypeService = contentTypeService;
            _publishedContentTypeFactory = publishedContentTypeFactory;

        }
        public IPublishedElement ConvertValueToContent(string id, string contentTypeAlias, string dataJson)
        {
            if (string.IsNullOrWhiteSpace(contentTypeAlias))
                return null;

            if (dataJson == null)
                return null;

            if (_umbracoContext.UmbracoContext == null)
                return ConvertValue(id, contentTypeAlias, dataJson);

            return (IPublishedElement)_appCaches.RequestCache.Get(
                $"Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.ConvertValueToContent_{id}_{contentTypeAlias}",
                () =>
                {
                    return ConvertValue(id, contentTypeAlias, dataJson);
                });
        }

        private IPublishedElement ConvertValue(string id, string contentTypeAlias, string dataJson)
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
                _dataEditors.TryGet(propType.EditorAlias, out var propEditor);
                var propPreValues = GetPreValuesCollectionByDataTypeId(propType.DataType.Id);

                var contentPropData = new ContentPropertyData(jProp.Value, propPreValues);

                var newValue = propEditor.GetValueEditor().FromEditor(contentPropData, jProp.Value);

                // Performing "ValueProcessing" if any ValueProcessor is configured for this Property Editor-alias.
                // TODO: Process values
                //var processorsCollection = Current.Factory.GetInstance<DocTypeGridEditorValueProcessorsCollection>();
                //var processor = processorsCollection.FirstOrDefault(x => x.IsProcessorFor(propEditor.Alias));
                //if (processor != null)
                //{
                //    newValue = processor.ProcessValue(newValue);
                //}

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
                    _logger.LogError(new EventId(0), ex, "[DocTypeGridEditor] Error creating Property object.");
                }

                if (prop2 != null)
                {
                    var newValue2 = propEditor.GetValueEditor().ConvertDbToString(propType2, newValue, _dataTypeService);

                    properties.Add(new DetachedPublishedProperty(propType, newValue2));
                }
            }

            // Manually parse out the special properties
            propValues.TryGetValue("name", out object nameObj);
            Guid.TryParse(id, out Guid key);

            // Get the current request node we are embedded in

            var pcr = _umbracoContext.UmbracoContext.PublishedRequest;
            var containerNode = pcr != null && pcr.HasPublishedContent() ? pcr.PublishedContent : null;

            // Create the model based on our implementation of IPublishedElement
            IPublishedElement content = new DetachedPublishedElement(
                key,
                contentTypes.PublishedContentType,
                properties.ToArray());

            if (_publishedModelFactory != null)
            {
                // Let the current model factory create a typed model to wrap our model
                content = _publishedModelFactory.CreateModel(content);
            }

            return content;

        }

        private object GetPreValuesCollectionByDataTypeId(int dataTypeId)
        {
            return (object)_appCaches.RuntimeCache.GetCacheItem(
                string.Concat(
                    "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_",
                    dataTypeId),
                () => _dataTypeService.GetDataType(dataTypeId).Configuration);
        }

        private ContentTypeContainer GetContentTypesByAlias(string contentTypeAlias)
        {
            if (Guid.TryParse(contentTypeAlias, out Guid contentTypeGuid))
                contentTypeAlias = GetContentTypeAliasByGuid(contentTypeGuid);

            return (ContentTypeContainer)_appCaches.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_", contentTypeAlias),
                () => new ContentTypeContainer
                {
                    PublishedContentType = new PublishedContentType(_contentTypeService.Get(contentTypeAlias), _publishedContentTypeFactory),
                    ContentType = _contentTypeService.Get(contentTypeAlias)
                });
        }

        private string GetContentTypeAliasByGuid(Guid contentTypeGuid)
        {
            return (string)_appCaches.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_", contentTypeGuid),
                () => _contentTypeService.GetAllContentTypeAliases(contentTypeGuid).FirstOrDefault());
        }
    }

    public class ContentTypeContainer
    {
        public PublishedContentType PublishedContentType { get; set; }

        public IContentType ContentType { get; set; }
    }
}
