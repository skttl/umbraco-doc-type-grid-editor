using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Our.Umbraco.DocTypeGridEditor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor.Controllers
{
    [PluginController("DocTypeGridEditorApi")]
    public class DocTypeGridEditorApiController : UmbracoAuthorizedJsonController
    {
        private readonly IUmbracoContextAccessor _umbracoContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentService _contentService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IPublishedRouter _router;
        private readonly DocTypeGridEditorHelper _dtgeHelper;
        private readonly ServiceContext _serviceContext;
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
        private readonly PropertyEditorCollection _propertyEditorCollection;

        public DocTypeGridEditorApiController(IUmbracoContextAccessor umbracoContext,
            IVariationContextAccessor variationContextAccessor,
            IContentTypeService contentTypeService,
            IContentService contentService,
            IDataTypeService dataTypeService,
            IShortStringHelper shortStringHelper,
            IPublishedContentQuery contentQuery,
            IPublishedRouter router,
            ServiceContext serviceContext,
            IPublishedContentTypeFactory publishedContentTypeFactory,
            PropertyEditorCollection propertyEditorCollection,
            DocTypeGridEditorHelper dtgeHelper)
        {
            _umbracoContext = umbracoContext;
            _variationContextAccessor = variationContextAccessor;
            _contentTypeService = contentTypeService;
            _contentService = contentService;
            _dataTypeService = dataTypeService;
            _shortStringHelper = shortStringHelper;
            _contentQuery = contentQuery;
            _router = router;
            _dtgeHelper = dtgeHelper;
            _serviceContext = serviceContext;
            _publishedContentTypeFactory = publishedContentTypeFactory;
            _propertyEditorCollection = propertyEditorCollection;
        }

        [HttpGet]
        public object GetContentTypeAliasByGuid([FromQuery] Guid guid)
        {
            return new
            {
                alias = _contentTypeService.GetAllContentTypeAliases(guid).FirstOrDefault()
            };
        }

        [HttpGet]
        public IEnumerable<object> GetContentTypes([FromQuery]string[] allowedContentTypes)
        {
            var allContentTypes = _contentTypeService.GetAll().ToList();
            var contentTypes = allContentTypes
                .Where(x => x.IsElement && x.VariesByCulture() == false)
                .Where(x => allowedContentTypes == null || allowedContentTypes.Length == 0 || allowedContentTypes.Any(y => Regex.IsMatch(x.Alias, y)))
                .OrderBy(x => x.Name)
                .ToList();

            var blueprints = _contentService.GetBlueprintsForContentTypes(contentTypes.Select(x => x.Id).ToArray()).ToArray();

            return contentTypes
                .Select(x => new
                {
                    id = x.Id,
                    guid = x.Key,
                    name = x.Name,
                    alias = x.Alias,
                    description = x.Description,
                    icon = x.Icon,
                    blueprints = blueprints.Where(bp => bp.ContentTypeId == x.Id).Select(bp => new
                    {
                        id = bp.Id,
                        name = bp.Name
                    })
                });
        }

        [HttpGet]
        public object GetContentType([FromQuery]string contentTypeAlias)
        {
            Guid docTypeGuid;
            if (Guid.TryParse(contentTypeAlias, out docTypeGuid))
                contentTypeAlias = _contentTypeService.GetAllContentTypeAliases(docTypeGuid).FirstOrDefault();

            var contentType = _contentTypeService.Get(contentTypeAlias);
            return new
            {
                icon = contentType != null ? contentType.Icon : "icon-science",
                title = contentType != null ? contentType.Name : "Doc Type",
                description = contentType != null ? contentType.Description : string.Empty
            };
        }

        [HttpGet]
        public object GetDataTypePreValues([FromQuery]string dtdId)
        {
            Guid guidDtdId;
            int intDtdId;

            IDataType dtd;

            // Parse the ID
            if (int.TryParse(dtdId, out intDtdId))
            {
                // Do nothing, we just want the int ID
                dtd = _dataTypeService.GetDataType(intDtdId);
            }
            else if (Guid.TryParse(dtdId, out guidDtdId))
            {
                dtd = _dataTypeService.GetDataType(guidDtdId);
            }
            else
            {
                return null;
            }

            if (dtd == null)
                return null;

            // Convert to editor config
            var dataType = _dataTypeService.GetDataType(dtd.Id);
            var propEditor = dataType.Editor;
            var content = propEditor.GetValueEditor().ConvertDbToString(new PropertyType(_shortStringHelper, dataType), dataType.Configuration);
            return content;
        }

        [HttpPost]
        public PartialViewResult GetPreviewMarkup([FromForm] PreviewData data, [FromQuery]int pageId)
        {
            var page = default(IPublishedContent);

            // If the page is new, then the ID will be zero
            if (pageId > 0)
            {
                // Get page container node
                page = _contentQuery.Content(pageId);
                if (page == null)
                {
                    // If unpublished, then fake PublishedContent
                    page = new UnpublishedContent(pageId, _contentService, _contentTypeService, _dataTypeService, _propertyEditorCollection, _publishedContentTypeFactory);
                }
            }


            if (_umbracoContext.GetRequiredUmbracoContext().PublishedRequest == null)
            {
                var request = _router.CreateRequestAsync(new Uri(Request.GetDisplayUrl())).Result;
                request.SetPublishedContent(page);
                _umbracoContext.GetRequiredUmbracoContext().PublishedRequest = request.Build();
            }

            // Set the culture for the preview
            if (page != null && page.Cultures != null)
            {
                var currentCulture = string.IsNullOrWhiteSpace(data.Culture) ? page.GetCultureFromDomains() : data.Culture;
                if (currentCulture != null && page.Cultures.ContainsKey(currentCulture))
                {
                    var culture = new CultureInfo(page.Cultures[currentCulture].Culture);
                    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                    _variationContextAccessor.VariationContext = new VariationContext(culture.Name);
                }
            }

            // Get content node object
            var content = _dtgeHelper.ConvertValueToContent(data.Id, data.ContentTypeAlias, data.Value);

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

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            viewData.Model = model;
            return new PartialViewResult()
            {
                ViewName = partialName,
                ViewData = viewData
            };
        }
    }
}
