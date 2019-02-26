using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Our.Umbraco.DocTypeGridEditor.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.DocTypeGridEditor.Web.Controllers
{
    [PluginController("DocTypeGridEditorApi")]
    public class DocTypeGridEditorApiController : UmbracoAuthorizedJsonController
    {
        private readonly IUmbracoContextAccessor _umbracoContext;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IPublishedContentCache _contentCache;

        public DocTypeGridEditorApiController()
        {
        }

        public DocTypeGridEditorApiController(IUmbracoContextAccessor umbracoContext,
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            IPublishedContentCache contentCache)
        {
            _umbracoContext = umbracoContext;
            _contentTypeService = contentTypeService;
            this._dataTypeService = dataTypeService;
            this._contentCache = contentCache;
        }

        [HttpGet]
        public object GetContentTypeAliasByGuid([ModelBinder] Guid guid)
        {
            return new
            {
                alias = _contentTypeService.GetAliasByGuid(guid)
            };
        }

        [HttpGet]
        public IEnumerable<object> GetContentTypes([ModelBinder] string[] allowedContentTypes)
        {
            var allContentTypes = Current.Services.ContentTypeService.GetAll().ToList();
            var contentTypes = allContentTypes
                .Where(x => allowedContentTypes == null || allowedContentTypes.Length == 0 || allowedContentTypes.Any(y => Regex.IsMatch(x.Alias, y)))
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.Id,
                    guid = x.Key,
                    name = x.Name,
                    alias = x.Alias,
                    description = x.Description,
                    icon = x.Icon
                });

            //foreach (var contentType in allContentTypes)
            //{
            //    var firstGroup = contentType.PropertyGroups.FirstOrDefault();
            //    if(firstGroup != null)
            //        foreach (var prop in firstGroup.PropertyTypes)
            //        {
            //            var test = GetDataTypePreValues(prop.DataTypeId.ToString());
            //        }
               
            //}

            return contentTypes;
        }

        [HttpGet]
        public object GetContentTypeIcon([ModelBinder] string contentTypeAlias)
        {
            Guid docTypeGuid;
            if (Guid.TryParse(contentTypeAlias, out docTypeGuid))
                contentTypeAlias = Services.ContentTypeService.GetAliasByGuid(docTypeGuid);

            var contentType = _contentTypeService.Get(contentTypeAlias);
            return new
            {
                icon = contentType != null ? contentType.Icon : string.Empty
            };
        }

        [HttpGet]
        public object GetDataTypePreValues(string dtdId)
        {
            Guid guidDtdId;
            int intDtdId;

            IDataType dtd;

            // Parse the ID
            if (int.TryParse(dtdId, out intDtdId))
            {
                // Do nothing, we just want the int ID
                dtd = Current.Services.DataTypeService.GetDataType(intDtdId);
            }
            else if (Guid.TryParse(dtdId, out guidDtdId))
            {
                dtd = Current.Services.DataTypeService.GetDataType(guidDtdId);
            }
            else
            {
                return null;
            }

            if (dtd == null)
                return null;

            // Convert to editor config
            //var preValue = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);
            //var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
            //return propEditor.PreValueEditor.ConvertDbToEditor(propEditor.DefaultPreValues, preValue);
            
            // TODO: FIXME! This is not the data we were expecting, not sure what we were actually expecting and why :)
            var dataType = Current.Services.DataTypeService.GetDataType(dtd.Id);
            var propEditor = dataType.Editor;
            var content = propEditor.GetValueEditor().ConvertDbToString(new PropertyType(dataType), dataType.Configuration, Current.Services.DataTypeService);
            return content;
        }

        [HttpPost]
        public HttpResponseMessage GetPreviewMarkup([FromBody] PreviewData data, [FromUri] int pageId)
        {
            var page = default(IPublishedContent);

            // If the page is new, then the ID will be zero
            if (pageId > 0)
            {
                // Get page container node
                page = _contentCache.GetById(pageId);
                if (page == null)
                {
                    // If unpublished, then fake PublishedContent
                    page = new UnpublishedContent(pageId, Services);
                }
            }

            // NOTE: The previous previewer had a content node associated with the request,
            // meaning that an implementation may have used this to traverse the content-tree.
            // In order to maintain backward-compatibility, we must ensure the PublishedContentRequest context.

            // TODO: Do we still need this hack?
            //if (UmbracoContext.PublishedContentRequest == null)
            //{
            //    UmbracoContext.PublishedContentRequest = new PublishedContentRequest(
            //        Request.RequestUri,
            //        UmbracoContext.RoutingContext,
            //        UmbracoConfig.For.UmbracoSettings().WebRouting,
            //        null)
            //    {
            //        PublishedContent = page
            //    };
            //}

            // Set the culture for the preview
            if (page != null)
            {
                var culture = new CultureInfo(page.GetCulture().Culture);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }

            // Get content node object
            var docTypeGridEditorHelper = new DocTypeGridEditorHelper(_umbracoContext);
            var content = docTypeGridEditorHelper.ConvertValueToContent(data.Id, data.ContentTypeAlias, data.Value);

            // Construct preview model
            var model = new PreviewModel
            {
                Page = page,
                Item = content,
                EditorAlias = data.EditorAlias,
                PreviewViewPath = data.PreviewViewPath,
                ViewPath = data.ViewPath
            };

            // Render view
            var partialName = "~/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditorPreviewer.cshtml";
            var markup = Helpers.ViewHelper.RenderPartial(partialName, model, UmbracoContext.HttpContext);

            // Return response
            var response = new HttpResponseMessage
            {
                Content = new StringContent(markup ?? string.Empty)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Html);

            return response;
        }
    }
}