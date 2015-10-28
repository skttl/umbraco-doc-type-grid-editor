using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.DocTypeGridEditor.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString RenderDocTypeGridEditorItem(this HtmlHelper helper,
            IPublishedContent content,
            string viewPath = "",
            string previewViewPath = "")
        { 
            if (content == null) 
                return new HtmlString(string.Empty);

            var controllerName = content.DocumentTypeAlias + "Surface"; 

            if (!string.IsNullOrWhiteSpace(viewPath)) 
                viewPath = viewPath.TrimEnd('/') + "/";

            if (!string.IsNullOrWhiteSpace(previewViewPath))
                previewViewPath = previewViewPath.TrimEnd('/') + "/";

            var actionName = content.DocumentTypeAlias;

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            if (umbracoHelper.SurfaceControllerExists(controllerName, actionName, true))
            {
                return helper.Action(actionName, controllerName, new
                {
                    dtgeModel = content,
                    dtgeViewPath = viewPath,
                    dtgePreviewViewPath = previewViewPath
                });
            }

            // Check for preview view 
            if (!string.IsNullOrWhiteSpace(previewViewPath)
                && helper.ViewContext.RequestContext.HttpContext.Request.QueryString["dtgePreview"] == "1")
            {
                var fullPreviewViewPath = previewViewPath + content.DocumentTypeAlias + ".cshtml";
                if (ViewEngines.Engines.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.Partial(fullPreviewViewPath, content);
                }
            }

            // Check for view path view
            if (!string.IsNullOrWhiteSpace(viewPath))
            {
                var fullViewPath = viewPath + content.DocumentTypeAlias + ".cshtml";
                if (ViewEngines.Engines.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.Partial(fullViewPath, content);
                }
            }

            // Resort to standard partial view
            return helper.Partial(content.DocumentTypeAlias, content);
        }
    }
}
