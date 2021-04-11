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
        public static bool ViewExists(ViewContext viewContext, string viewName, bool isPartial = false)
        {
            var viewEngine = viewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            var result = viewEngine.GetView(null, viewName, true).Success || viewEngine.GetView(null, viewName, false).Success;
            return result;
        }
    }
}
