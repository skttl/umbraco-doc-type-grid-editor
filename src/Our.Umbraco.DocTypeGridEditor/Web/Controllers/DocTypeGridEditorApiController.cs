using System;
using System.Collections.Generic;
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
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;

namespace Our.Umbraco.DocTypeGridEditor.Web.Controllers
{
    [PluginController("DocTypeGridEditorApi")]
    public class DocTypeGridEditorApiController : UmbracoAuthorizedJsonController
    {
        [HttpGet]
        public object GetContentTypeAliasByGuid([ModelBinder] Guid guid)
        {
            return new
            {
                alias = Services.ContentTypeService.GetAliasByGuid(guid)
            };
        }

        [HttpGet]
        public IEnumerable<object> GetContentTypes([ModelBinder] string[] allowedContentTypes)
        {
            return Services.ContentTypeService.GetAllContentTypes()
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
        }

        [HttpGet]
        public object GetContentTypeIcon([ModelBinder] string contentTypeAlias)
        {
            Guid docTypeGuid;
            if (Guid.TryParse(contentTypeAlias, out docTypeGuid))
                contentTypeAlias = Services.ContentTypeService.GetAliasByGuid(docTypeGuid);

            var contentType = Services.ContentTypeService.GetContentType(contentTypeAlias);
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

            IDataTypeDefinition dtd;

            // Parse the ID
            if (int.TryParse(dtdId, out intDtdId))
            {
                // Do nothing, we just want the int ID
                dtd = Services.DataTypeService.GetDataTypeDefinitionById(intDtdId);
            }
            else if (Guid.TryParse(dtdId, out guidDtdId))
            {
                dtd = Services.DataTypeService.GetDataTypeDefinitionById(guidDtdId);
            }
            else
            {
                return null;
            }

            if (dtd == null)
                return null;

            // Convert to editor config
            var preValue = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);
            var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
            return propEditor.PreValueEditor.ConvertDbToEditor(propEditor.DefaultPreValues, preValue);
        }

        [HttpPost]
        public HttpResponseMessage GetPreviewMarkup([FromBody] PreviewData data, [FromUri] int pageId)
        {
            // Set DTGE's preview to be in "preview mode", (storing the original value in a temp variable for resetting it later).
            var inPreviewMode = UmbracoContext.InPreviewMode;
            UmbracoContext.InPreviewMode = true;

            var page = default(IPublishedContent);

            // If the page is new, then the ID will be zero
            if (pageId > 0)
            {
                // Get page container node
                page = UmbracoContext.ContentCache.GetById(pageId);
                if (page == null)
                {
                    // If unpublished, then fake PublishedContent
                    page = new UnpublishedContent(pageId, Services);
                }
            }

            // NOTE: The previous previewer had a content node associated with the request,
            // meaning that an implementation may have used this to traverse the content-tree.
            // In order to maintain backward-compatibility, we must ensure the PublishedContentRequest context.
            if (UmbracoContext.PublishedContentRequest == null)
            {
                UmbracoContext.PublishedContentRequest = new PublishedContentRequest(
                    Request.RequestUri,
                    UmbracoContext.RoutingContext,
                    UmbracoConfig.For.UmbracoSettings().WebRouting,
                    null)
                {
                    PublishedContent = page
                };
            }

            // Set the culture for the preview
            if (page != null)
            {
                var culture = page.GetCulture();
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }

            // Get content node object
            var content = DocTypeGridEditorHelper.ConvertValueToContent(data.Id, data.ContentTypeAlias, data.Value);

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

            // Restore the "preview mode" to its original value
            UmbracoContext.InPreviewMode = inPreviewMode;

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