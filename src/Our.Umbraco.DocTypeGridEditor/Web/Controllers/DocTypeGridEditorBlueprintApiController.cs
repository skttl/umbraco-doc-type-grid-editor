using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Our.Umbraco.DocTypeGridEditor.Models;
using Serilog.Core;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing.CompositionExtensions;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Editors;
using Umbraco.Web.Editors.Binders;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.WebApi;
using Umbraco.Web.WebApi.Filters;


namespace Our.Umbraco.DocTypeGridEditor.Web.Controllers
{
    [PluginController("DocTypeGridEditorApi")]
    public class DocTypeGridEditorBlueprintApiController : UmbracoAuthorizedApiController
    {
        private readonly IUmbracoContextAccessor _umbracoContext;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IPublishedContentCache _contentCache;
        private ContentController _contentController;

        public DocTypeGridEditorBlueprintApiController(IUmbracoContextAccessor umbracoContext,
            IScopeProvider scopeProvider,
            ISqlContext sqlContext,
            IProfilingLogger logger,
            IRuntimeState runtimeState
            )
        {
            var propertyEditorCollection = new PropertyEditorCollection(new DataEditorCollection(Enumerable.Empty<DataEditor>()));
            _contentController = new ContentController(propertyEditorCollection, GlobalSettings, umbracoContext, sqlContext, Services, AppCaches, logger, runtimeState, Umbraco, scopeProvider);
        }

        [FileUploadCleanupFilter]
        [HttpPost]
        public ContentItemDisplay PostSaveBlueprint([ModelBinder(typeof(BlueprintItemBinder))] ContentItemSave contentItem)
        {
            return _contentController.PostSaveBlueprint(contentItem);
        }
        [HttpDelete]
        [HttpPost]
        public HttpResponseMessage DeleteBlueprint(int id)
        {
            var found = Services.ContentService.GetBlueprintById(id);

            if (found == null)
            {
                Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Services.ContentService.DeleteBlueprint(found);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
