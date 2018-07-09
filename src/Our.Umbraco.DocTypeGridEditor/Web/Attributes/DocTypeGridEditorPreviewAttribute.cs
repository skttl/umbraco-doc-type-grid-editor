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
                // NOTE: [LK:2016-11-16] If the preview result is a redirect, then cancel the request.
                // The issue here is that typically a redirect from a controller would be to a full-loading HTML page.
                // Meaning that a full bodied HTML page would be injected into a DTGE grid cell, causing havoc.
                // This is a temporary resolution until we implement a more robust DTGE previewer mechanism.
                else if (filterContext.Result is RedirectResult)
                {
                    filterContext.Cancel = true;
                }
            }

            base.OnResultExecuting(filterContext);
        }
    }
}