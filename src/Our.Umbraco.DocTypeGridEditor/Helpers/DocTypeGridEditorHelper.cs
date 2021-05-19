using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.DocTypeGridEditor.Config;
using Our.Umbraco.DocTypeGridEditor.Models;
using Our.Umbraco.DocTypeGridEditor.ValueProcessing.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
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
        private readonly IViewComponentSelector _viewComponentSelector;
        private readonly DocTypeGridEditorValueProcessorsCollection _docTypeGridEditorValueProcessorsCollection;
        private readonly DocTypeGridEditorSettings _options;

        public DocTypeGridEditorHelper(
            AppCaches appCaches,
            IUmbracoContextAccessor umbracoContextAccessor,
            PropertyEditorCollection dataEditors,
            ILoggerFactory loggerFactory,
            IDataTypeService dataTypeService,
            IPublishedModelFactory publishedModelFactory,
            IContentTypeService contentTypeService,
            IPublishedContentTypeFactory publishedContentTypeFactory,
            IViewComponentSelector viewComponentSelector,
            DocTypeGridEditorValueProcessorsCollection docTypeGridEditorValueProcessorsCollection,
            IOptions<DocTypeGridEditorSettings> options)
        {
            _appCaches = appCaches;
            _umbracoContext = umbracoContextAccessor;
            _dataEditors = dataEditors;
            _logger = loggerFactory.CreateLogger<DocTypeGridEditorHelper>();
            _dataTypeService = dataTypeService;
            _publishedModelFactory = publishedModelFactory;
            _contentTypeService = contentTypeService;
            _publishedContentTypeFactory = publishedContentTypeFactory;
            _viewComponentSelector = viewComponentSelector;
            _docTypeGridEditorValueProcessorsCollection = docTypeGridEditorValueProcessorsCollection;
            _options = options.Value;

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
                var processor = _docTypeGridEditorValueProcessorsCollection.FirstOrDefault(x => x.IsProcessorFor(propEditor.Alias));
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
            return _appCaches.RuntimeCache.GetCacheItem(
                string.Concat(
                    "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_",
                    dataTypeId),
                () => _dataTypeService.GetDataType(dataTypeId).Configuration);
        }

        private ContentTypeContainer GetContentTypesByAlias(string contentTypeAlias)
        {
            if (Guid.TryParse(contentTypeAlias, out Guid contentTypeGuid))
                contentTypeAlias = GetContentTypeAliasByGuid(contentTypeGuid);

            return _appCaches.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_", contentTypeAlias),
                () => new ContentTypeContainer
                {
                    PublishedContentType = new PublishedContentType(_contentTypeService.Get(contentTypeAlias), _publishedContentTypeFactory),
                    ContentType = _contentTypeService.Get(contentTypeAlias)
                });
        }

        private string GetContentTypeAliasByGuid(Guid contentTypeGuid)
        {
            return _appCaches.RuntimeCache.GetCacheItem(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_", contentTypeGuid),
                () => _contentTypeService.GetAllContentTypeAliases(contentTypeGuid).FirstOrDefault());
        }

        public IHtmlContent RenderDocTypeGridEditorItem(
            IViewComponentHelper helper,
            IHtmlHelper htmlHelper,
            dynamic model)
        {
            if (model.value != null)
            {
                string id = model.value.id.ToString();
                string editorAlias = model.editor.alias.ToString();
                string contentTypeAlias = (model.value.dtgeContentTypeAlias ?? model.value.docType).ToString();
                string value = model.value.value.ToString();
                string viewPath = model.editor.config.viewPath.ToString();

                var content = ConvertValue(id, contentTypeAlias, value);
                return RenderDocTypeGridEditorItem(helper, htmlHelper, content, editorAlias, viewPath);
            }

            return null;
        }

        public IHtmlContent RenderDocTypeGridEditorItem(
            IViewComponentHelper helper,
            IHtmlHelper htmlHelper,
            IPublishedElement content,
            string editorAlias = "",
            string viewPath = "",
            string previewViewPath = "",
            bool isPreview = false)
        {
            if (content == null)
                return new HtmlString("<pre>content is null</pre>");

            // get view path
            if (!TryGetViewPath(htmlHelper.ViewContext, editorAlias, content.ContentType.Alias, viewPath, previewViewPath, isPreview, out string fullViewPath))
            {
                return new HtmlString($"<pre>could not get viewpath. {editorAlias}, {content.ContentType.Alias}, {viewPath}, {previewViewPath}, {isPreview}, {fullViewPath}</pre>");
            }

            var renderParams = new { model = content, viewPath = fullViewPath };
            var viewComponent = _options.DefaultDocTypeGridEditorViewComponent;

            if (!TryGetComponentName(new[] { editorAlias, content.ContentType.Alias }, out string componentName))
            {
                return new HtmlString($"<pre>could not get componentName. {editorAlias}, {content.ContentType.Alias}, {componentName}</pre>");

            }

            return helper.InvokeAsync(componentName, renderParams).Result;
        }

        private bool TryGetViewPath(ViewContext viewContext, string editorAlias, string contentTypeAlias, string viewPath, string previewViewPath, bool isPreview, out string fullViewPath)
        {
            fullViewPath = "";

            if (isPreview && !previewViewPath.IsNullOrWhiteSpace())
            {
                previewViewPath = previewViewPath.EnsureEndsWith('/');
                fullViewPath = $"{previewViewPath}{editorAlias}.cshtml";
                if (!ViewExists(viewContext, fullViewPath))
                {
                    fullViewPath = $"{previewViewPath}{contentTypeAlias}.cshtml";
                    if (!ViewExists(viewContext, fullViewPath))
                    {
                        fullViewPath = $"{previewViewPath}Default.cshtml";
                        if (!ViewExists(viewContext, fullViewPath))
                        {
                            fullViewPath = "";
                        }
                    }
                }
            }
            if (!viewPath.IsNullOrWhiteSpace() && (fullViewPath.IsNullOrWhiteSpace() || !isPreview))
            {
                viewPath = viewPath.EnsureEndsWith('/');
                fullViewPath = $"{viewPath}{editorAlias}.cshtml";
                if (!ViewExists(viewContext, fullViewPath))
                {
                    fullViewPath = $"{viewPath}{contentTypeAlias}.cshtml";
                    if (!ViewExists(viewContext, fullViewPath))
                    {
                        fullViewPath = $"{viewPath}Default.cshtml";
                        if (!ViewExists(viewContext, fullViewPath))
                        {
                            fullViewPath = "";
                        }
                    }
                }
            }

            return !fullViewPath.IsNullOrWhiteSpace();
        }

        private bool TryGetComponentName(string[] names, out string componentName)
        {
            componentName = "";
            foreach (var name in names)
            {
                Console.WriteLine($"getting component name for {name}");
                if (_viewComponentSelector.SelectComponent($"{name}DocTypeGridEditor") != null)
                {
                    componentName = $"{name}DocTypeGridEditor";
                    return true;
                }
            }
            return false;
        }

        public static bool ViewExists(ViewContext viewContext, string viewName)
        {
            var viewEngine = viewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            var result = viewEngine.GetView(null, viewName, true).Success || viewEngine.GetView(null, viewName, false).Success;
            return result;
        }
    }
}
