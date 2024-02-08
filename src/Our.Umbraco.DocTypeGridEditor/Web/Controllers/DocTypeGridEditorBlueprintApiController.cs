using System.Web.Http.ModelBinding;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Editors.Binders;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi.Filters;

namespace Our.Umbraco.DocTypeGridEditor.Web.Controllers
{
    [PluginController("DocTypeGridEditorBlueprintApi")]
    public class DocTypeGridEditorBlueprintApiController : ContentControllerBase
    {
        private ContentController _contentController;

        public DocTypeGridEditorBlueprintApiController(PropertyEditorCollection propertyEditors, IScopeProvider scopeProvider, IGlobalSettings globalSettings, IUmbracoContextAccessor umbracoContextAccessor, ISqlContext sqlContext, ServiceContext services, AppCaches appCaches, IProfilingLogger logger, IRuntimeState runtimeState, UmbracoHelper umbracoHelper) : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            _contentController = new ContentController(propertyEditors, globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper, scopeProvider);
        }

        /// <summary>
        /// Saves content
        /// </summary>
        /// <returns></returns>



        [FileUploadCleanupFilter]
        public ContentItemDisplay PostSaveBlueprint([ModelBinder(typeof(BlueprintItemBinder))] ContentItemSave contentItem)
        {
            return _contentController.PostSaveBlueprint(contentItem);
        }

    }
}
