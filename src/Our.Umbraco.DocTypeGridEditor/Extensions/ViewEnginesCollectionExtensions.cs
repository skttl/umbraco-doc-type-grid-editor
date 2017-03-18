using System.Web.Mvc;

namespace Our.Umbraco.DocTypeGridEditor.Extensions
{
    internal static class ViewEnginesCollectionExtensions
    {
        public static bool ViewExists(
            this ViewEngineCollection viewEngines,
            ControllerContext controllerContext,
            string viewName, bool isPartial = false)
        {
            var result = !isPartial
                ? viewEngines.FindView(controllerContext, viewName, null)
                : viewEngines.FindPartialView(controllerContext, viewName);

            if (result.View != null)
                return true;

            return false;
        }
    }
}