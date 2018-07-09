using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Our.Umbraco.DocTypeGridEditor.Models;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

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
        public HttpResponseMessage GetPreviewMarkup([FromBody] FormDataCollection item, [FromUri] int nodeId)
        {
            // Get page container node
            var page = UmbracoContext.ContentCache.GetById(nodeId);
            if (page == null)
            {
                // If unpublished, then fake PublishedContent (with IContent object)
                page = new UnpublishedContent(nodeId, Services);
            }

            var culture = UmbracoContext.Application.Services.ContentService.GetById(nodeId).GetCulture();
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            // Construct preview model
            var model = new PreviewModel { Page = page, Values = item };

            // Render view
            var markup = Helpers.ViewHelper.RenderPartial("~/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditorPreviewer.cshtml", model);

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