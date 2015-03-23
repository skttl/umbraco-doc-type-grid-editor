using System.Web.Mvc;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.DocTypeGridEditor.Web.Controllers
{
    public abstract class DocTypeGridEditorSurfaceController 
        : DocTypeGridEditorSurfaceController<IPublishedContent>
    { }

    public abstract class DocTypeGridEditorSurfaceController<TModel> : SurfaceController
    {
        public TModel Model
        {
            get { return (TModel)ControllerContext.RouteData.Values["dtgeModel"]; }
        }

        public string ViewPath
        {
            get { return ControllerContext.RouteData.Values["dtgeViewPath"] as string ?? string.Empty; }
        }

        public string PreviewViewPath
        {
            get { return ControllerContext.RouteData.Values["dtgePreviewViewPath"] as string ?? string.Empty; }
        }

        public bool IsPreview
        {
            get { return Request.QueryString["dtgePreview"] == "1"; }
        }

        protected ActionResult CurrentPartialView(object model = null)
        {
            if (model == null)
                model = Model;

            var viewName = ControllerContext.RouteData.Values["action"].ToString();

            var previewViewPath = GetFullViewPath(viewName, ViewPath);
            if (IsPreview && ViewEngines.Engines.ViewExists(ControllerContext, previewViewPath, true))
                return base.PartialView(previewViewPath, model);

            var viewPath = GetFullViewPath(viewName, ViewPath);
            if (ViewEngines.Engines.ViewExists(ControllerContext, viewPath, true))
                return base.PartialView(viewPath, model);

            return HttpNotFound();
        }

        protected new PartialViewResult PartialView(string viewName)
        {
            return PartialView(viewName, ViewPath, Model);
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            return PartialView(viewName, ViewPath, model);
        }

        protected PartialViewResult PartialView(string viewName, string baseViewPath, object model)
        {
            var fullViewPath = GetFullViewPath(viewName, baseViewPath);
            return base.PartialView(fullViewPath, model);
        }

        protected string GetFullViewPath(string viewName, string baseViewPath)
        {
            if (viewName.StartsWith("~") || viewName.StartsWith("/")
                || viewName.StartsWith(".") || string.IsNullOrWhiteSpace(baseViewPath))
            {
                return viewName;
            }

            return baseViewPath.TrimEnd('/') + "/" + viewName + ".cshtml";
        }
        
    }
}
