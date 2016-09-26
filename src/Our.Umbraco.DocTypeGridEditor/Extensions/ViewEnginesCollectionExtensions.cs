using System.Web.Mvc;
using Umbraco.Core.Logging;

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

            LogHelper.Info<ViewEngineCollection>("[DocTypeGridEditor] No view file found with the name '{0}'", () => viewName);

            return false;
        }
    }
}