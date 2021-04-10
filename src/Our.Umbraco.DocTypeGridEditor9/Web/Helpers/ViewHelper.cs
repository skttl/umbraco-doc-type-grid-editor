using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Cms.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;

namespace Our.Umbraco.DocTypeGridEditor9.Web.Helpers
{
    internal static class ViewHelper
    {
        private class DummyController : Controller { }

        public static string RenderPartial(string partialName, object model, HttpContext httpContext = null, IUmbracoContext umbracoContext = null)
        {
            using (var sw = new StringWriter())
            {
                if (httpContext == null)
                    return null;
                // TODO: Can we make a HttpContext?
                // httpContext = new HttpContextWrapper(HttpContext.Current);

                if (umbracoContext == null)
                    return null;
                // TODO: Can we get the UmbracoContext?
                // umbracoContext = Current.UmbracoContext;

                return null;
                /*
                 * TODO: Don't know how to do the rest
                var routeData = new RouteData();
                routeData.Values.Add("controller", "DummyController");

                if (umbracoContext.PublishedRequest != null)
                {
                    routeData.DataTokens[Constants.UmbracoRouteDefinitionDataToken] = new RouteDefinition
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
                */
            }
        }

        public static bool ViewExists(ViewContext viewContext, string viewName, bool isPartial = false)
        {
            /* TODO: This doesn't work
            var result = isPartial == false
                ? viewContext..Engines.FindView(viewContext, viewName, null)
                : ViewEngines.Engines.FindPartialView(viewContext, viewName);

            if (result.View != null)
                return true;

            return false;*/
            return true;
        }
    }
}
