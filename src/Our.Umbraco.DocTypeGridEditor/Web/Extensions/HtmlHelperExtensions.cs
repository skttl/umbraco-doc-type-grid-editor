using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Content = System.Web.UI.WebControls.Content;

namespace Our.Umbraco.DocTypeGridEditor.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString RenderDocTypeGridEditorItem(this HtmlHelper helper,
            IPublishedContent content,
            string editorAlias = "",
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

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            if (!editorAlias.IsNullOrWhiteSpace() && umbracoHelper.SurfaceControllerExists(controllerName, editorAlias, true))
            {
                return helper.Action(editorAlias, controllerName, new
                {
                    dtgeModel = content,
                    dtgeViewPath = viewPath,
                    dtgePreviewViewPath = previewViewPath
                });
            }

            if (umbracoHelper.SurfaceControllerExists(controllerName, content.DocumentTypeAlias, true))
            {
                return helper.Action(content.DocumentTypeAlias, controllerName, new
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
                var fullPreviewViewPath = previewViewPath + editorAlias + ".cshtml";
                if (ViewEngines.Engines.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.Partial(fullPreviewViewPath, content);
                }

                fullPreviewViewPath = previewViewPath + content.DocumentTypeAlias + ".cshtml";
                if (ViewEngines.Engines.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.Partial(fullPreviewViewPath, content);
                }
            }

            // Check for view path view
            if (!string.IsNullOrWhiteSpace(viewPath))
            {
                var fullViewPath = viewPath + editorAlias + ".cshtml";
                if (ViewEngines.Engines.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.Partial(fullViewPath, content);
                }

                fullViewPath = viewPath + content.DocumentTypeAlias + ".cshtml";
                if (ViewEngines.Engines.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.Partial(fullViewPath, content);
                }
            }

            // Resort to standard partial views
            if (ViewEngines.Engines.ViewExists(helper.ViewContext, editorAlias, true))
            {
                return helper.Partial(editorAlias, content);
            }

            return helper.Partial(content.DocumentTypeAlias, content);
        }
    }
}
