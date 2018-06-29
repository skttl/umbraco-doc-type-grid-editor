using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Logging;

namespace Our.Umbraco.DocTypeGridEditor.Web.Helpers
{
    public static class ViewHelper
    {
        private class DummyController : Controller { }

        internal static string RenderPartial(string partialName, object model)
        {
            using (var sw = new StringWriter())
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);

                var routeData = new RouteData();
                routeData.Values.Add("controller", "DummyController");

                var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new DummyController());

                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, partialName);
                if (viewResult.View == null)
                {
                    LogHelper.Warn(typeof(ViewHelper), $"No view found for partial '{partialName}'");
                    return null;
                }

                viewResult.View.Render(new ViewContext(controllerContext, viewResult.View, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

                return sw.ToString();
            }
        }
    }
}
