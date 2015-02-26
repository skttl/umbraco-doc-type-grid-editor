using System.Web.Mvc;
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

        protected ActionResult CurrentPartialView(object model = null)
        {
            if (model == null)
                model = Model;

            var viewName = ControllerContext.RouteData.Values["action"].ToString();
            var viewPath = GetFullViewPath(viewName);

            if (ViewExists(viewPath, true))
                return base.PartialView(viewPath, model);

            return HttpNotFound();
        }

        protected new PartialViewResult PartialView(string viewName)
        {
            return PartialView(viewName, Model);
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            var viewPath = GetFullViewPath(viewName);
            return base.PartialView(viewPath, model);
        }

        protected string GetFullViewPath(string viewName)
        {
            if (viewName.StartsWith("~") || viewName.StartsWith("/")
                || viewName.StartsWith(".") || string.IsNullOrWhiteSpace(ViewPath))
            {
                return viewName;
            }

            return ViewPath.TrimEnd('/') + "/" + viewName + ".cshtml";
        }

        protected bool ViewExists(string viewName, bool isPartial = false)
        {
            var result = !isPartial
                ? ViewEngines.Engines.FindView(ControllerContext, viewName, null)
                : ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
            if (result.View != null)
                return true;

            LogHelper.Warn<DocTypeGridEditorSurfaceController>("No view file found with the name " + viewName);
            return false;
        }
    }
}
