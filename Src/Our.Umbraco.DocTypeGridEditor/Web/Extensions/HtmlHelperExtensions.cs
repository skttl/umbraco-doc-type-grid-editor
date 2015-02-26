using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.DocTypeGridEditor.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString RenderDocTypeGridEditorItem(this HtmlHelper helper,
            string id,
            string docType,
            string value,
            string viewPath = "",
            string actionName = "",
            object model = null)
        {
            var content = DocTypeGridEditorHelper.ConvertValueToContent(id, docType, value);
            return helper.RenderDocTypeGridEditorItem(content, viewPath, actionName, model);
        }

        public static HtmlString RenderDocTypeGridEditorItem(this HtmlHelper helper,
            IPublishedContent content,
            string viewPath = "",
            string actionName = "",
            object model = null)
        {
            
            if (content == null)
                return new HtmlString(string.Empty);

            var controllerName = content.DocumentTypeAlias + "Surface";

            if (!string.IsNullOrWhiteSpace(viewPath))
                viewPath = viewPath.TrimEnd('/') + "/";

            if (string.IsNullOrWhiteSpace(actionName))
                actionName = content.DocumentTypeAlias;

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            if (umbracoHelper.SurfaceControllerExists(controllerName, actionName, true))
            {
                return helper.Action(actionName, controllerName, new
                {
                    dtgeModel = model ?? content,
                    dtgeViewPath = viewPath
                });
            }

            if (!string.IsNullOrWhiteSpace(viewPath))
                return helper.Partial(viewPath + content.DocumentTypeAlias + ".cshtml", content);

            return helper.Partial(content.DocumentTypeAlias, content);
        }
    }
}
