using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Mvc;
using UmbracoWebConstants = Umbraco.Core.Constants.Web;

namespace Our.Umbraco.DocTypeGridEditor.Web.Helpers
{
    internal static class ViewHelper
    {
        private class DummyController : Controller { }

        public static string RenderPartial(string partialName, object model, HttpContextBase httpContext = null, UmbracoContext umbracoContext = null)
        {
            using (var sw = new StringWriter())
            {
                if (httpContext == null)
                    httpContext = new HttpContextWrapper(HttpContext.Current);

                if (umbracoContext == null)
                    umbracoContext = Current.UmbracoContext;

                var routeData = new RouteData();
                routeData.Values.Add("controller", "DummyController");

                if (umbracoContext.PublishedRequest != null)
                {
                    routeData.DataTokens[UmbracoWebConstants.UmbracoRouteDefinitionDataToken] = new RouteDefinition
                    {
                        PublishedRequest = umbracoContext.PublishedRequest
                    };
                }

                var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new DummyController());

                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, partialName);
                if (viewResult.View == null)
                {
                    Current.Logger.Warn(typeof(ViewHelper), $"No view found for partial '{partialName}'");
                    return null;
                }

                viewResult.View.Render(new ViewContext(controllerContext, viewResult.View, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

                return sw.ToString();
            }
        }

        public static bool ViewExists(ControllerContext controllerContext, string viewName, bool isPartial = false)
        {
            var result = isPartial == false
                ? ViewEngines.Engines.FindView(controllerContext, viewName, null)
                : ViewEngines.Engines.FindPartialView(controllerContext, viewName);

            if (result.View != null)
                return true;

            return false;
        }
    }
}