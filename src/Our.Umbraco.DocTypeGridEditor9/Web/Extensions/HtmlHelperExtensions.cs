using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Our.Umbraco.DocTypeGridEditor9.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor9.Web.Extensions
{

    public static class HtmlHelperExtensions
    {
        public static IHtmlContent RenderDocTypeGridEditorItem(
            this IHtmlHelper helper,
            IPublishedElement content,
            string editorAlias = "",
            string viewPath = "",
            string previewViewPath = "",
            bool isPreview = false)
        {
            if (content == null)
                return new HtmlString(string.Empty);


            if (string.IsNullOrWhiteSpace(viewPath) == false)
                viewPath = viewPath.EnsureEndsWith('/');

            if (string.IsNullOrWhiteSpace(previewViewPath) == false)
                previewViewPath = previewViewPath.EnsureEndsWith('/');

            /*
             * TODO: Make the surface controller logic work
             *
            var controllerName = $"{content.ContentType.Alias}Surface";
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
            */

            // Check for preview view
            if (string.IsNullOrWhiteSpace(previewViewPath) == false && isPreview)
            {
                var fullPreviewViewPath = $"{previewViewPath}{editorAlias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.PartialAsync(fullPreviewViewPath, content).Result;
                }

                fullPreviewViewPath = $"{previewViewPath}{content.ContentType.Alias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.PartialAsync(fullPreviewViewPath, content).Result;
                }

                fullPreviewViewPath = $"{previewViewPath}Default.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullPreviewViewPath, true))
                {
                    return helper.PartialAsync(fullPreviewViewPath, content).Result;
                }
            }

            // Check for view path view
            if (string.IsNullOrWhiteSpace(viewPath) == false)
            {
                var fullViewPath = $"{viewPath}{editorAlias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.PartialAsync(fullViewPath, content).Result;
                }

                fullViewPath = $"{viewPath}{content.ContentType.Alias}.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.PartialAsync(fullViewPath, content).Result;
                }

                fullViewPath = $"{viewPath}Default.cshtml";
                if (ViewHelper.ViewExists(helper.ViewContext, fullViewPath, true))
                {
                    return helper.PartialAsync(fullViewPath, content).Result;
                }
            }

            // Resort to standard partial views
            if (ViewHelper.ViewExists(helper.ViewContext, editorAlias, true))
            {
                return helper.PartialAsync(editorAlias, content).Result;
            }

            return helper.PartialAsync(content.ContentType.Alias, content).Result;
        }

    }
}
