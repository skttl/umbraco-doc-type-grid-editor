using Our.Umbraco.DocTypeGridEditor.Composing;
using Our.Umbraco.DocTypeGridEditor.Web.Helpers;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IPublishedContent GetContainerPage(this HtmlHelper helper)
        {
            //return helper.ViewData["DTGE_ContainerPage"] as IPublishedContent;
            if (helper.ViewContext.TempData.ContainsKey("dtgeContainerPage"))
            {
                return helper.ViewContext.TempData["dtgeContainerPage"] as IPublishedContent;
            }
            return null;
        }

        public static HtmlString RenderDocTypeGridEditorItem(
            this HtmlHelper helper,
            IPublishedElement content,
            IPublishedContent containerPage,
            string editorAlias = "",
            string viewPath = "",
            string previewViewPath = "",
            bool isPreview = false)
        {
            if (content == null)
                return new HtmlString(string.Empty);

            var controllerName = $"{content.ContentType.Alias}Surface";

            if (string.IsNullOrWhiteSpace(viewPath) == false)
                viewPath = viewPath.EnsureEndsWith('/');

            if (string.IsNullOrWhiteSpace(previewViewPath) == false)
                previewViewPath = previewViewPath.EnsureEndsWith('/');

            //Store the container page reference away for later
            if (containerPage != null)
            {
                helper.ViewContext.TempData["dtgeContainerPage"] = containerPage;
            }

            var routeValues = new
            {
                dtgeModel = content,
                dtgeViewPath = viewPath,
                dtgePreviewViewPath = previewViewPath,
                dtgePreview = isPreview
            };

            // Try looking for surface controller with action named after the editor alias
            if (string.IsNullOrWhiteSpace(editorAlias) == false && SurfaceControllerHelper.SurfaceControllerExists(controllerName, editorAlias, true))
            {
                return helper.Action(editorAlias, controllerName, routeValues);
            }

            // Try looking for surface controller with action named after the doc type alias alias
            if (SurfaceControllerHelper.SurfaceControllerExists(controllerName, content.ContentType.Alias, true))
            {
                return helper.Action(content.ContentType.Alias, controllerName, routeValues);
            }

            //// See if a default surface controller has been registered
            var defaultController = Current.DefaultDocTypeGridEditorSurfaceControllerType;
            if (defaultController != null)
            {
                var defaultControllerName = defaultController.Name.Substring(0, defaultController.Name.LastIndexOf("Controller"));

                // Try looking for an action named after the editor alias
                if (string.IsNullOrWhiteSpace(editorAlias) == false && SurfaceControllerHelper.SurfaceControllerExists(defaultControllerName, editorAlias, true))
                {
                    return helper.Action(editorAlias, defaultControllerName, routeValues);
                }

                // Try looking for a doc type alias action
                if (SurfaceControllerHelper.SurfaceControllerExists(defaultControllerName, content.ContentType.Alias, true))
                {
                    return helper.Action(content.ContentType.Alias, defaultControllerName, routeValues);
                }

                // Just go with a default action name
                return helper.Action("Index", defaultControllerName, routeValues);
            }

            // Check for preview view
            if (string.IsNullOrWhiteSpace(previewViewPath) == false && isPreview)
            {
                var fullPreviewViewPath = $"{previewViewPath}{editorAlias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.Partial(fullPreviewViewPath, content);
                }

                fullPreviewViewPath = $"{previewViewPath}{content.ContentType.Alias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.Partial(fullPreviewViewPath, content);
                }

                fullPreviewViewPath = $"{previewViewPath}Default.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.Partial(fullPreviewViewPath, content);
                }
            }

            // Check for view path view
            if (string.IsNullOrWhiteSpace(viewPath) == false)
            {
                var fullViewPath = $"{viewPath}{editorAlias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.Partial(fullViewPath, content);
                }

                fullViewPath = $"{viewPath}{content.ContentType.Alias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.Partial(fullViewPath, content);
                }

                fullViewPath = $"{viewPath}Default.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.Partial(fullViewPath, content);
                }
            }

            // Resort to standard partial views
            if (ViewHelper.ViewExists(helper.ViewContext, editorAlias, true))
            {
                return helper.Partial(editorAlias, content);
            }

            return helper.Partial(content.ContentType.Alias, content);
        }
    }
}