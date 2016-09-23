using System.Web;
using System.Web.Mvc;

namespace Our.Umbraco.DocTypeGridEditor.Web.Attributes
{
    internal class DocTypeGridEditorPreviewAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (HttpContext.Current.Request.QueryString["dtgePreview"] == "1"
                && HttpContext.Current.Request.HttpMethod == "POST")
            {
                var viewResult = filterContext.Result as ViewResult;
                if (viewResult != null)
                {
                    viewResult.ViewName =
                        "~/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditorPreviewer.cshtml";
                }
            }

            base.OnResultExecuting(filterContext);
        }
    }
}