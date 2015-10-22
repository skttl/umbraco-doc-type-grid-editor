using System.Web.Mvc;
using Our.Umbraco.DocTypeGridEditor.Web.Attributes;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Our.Umbraco.DocTypeGridEditor
{
    internal class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            GlobalFilters.Filters.Add(new DocTypeGridEditorPreviewAttribute());
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            DataTypeService.Saved += ExpireCache;
        }

        private void ExpireCache(IDataTypeService sender, global::Umbraco.Core.Events.SaveEventArgs<global::Umbraco.Core.Models.IDataTypeDefinition> e)
        {
            foreach (var dataType in e.SavedEntities)
            {
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(
                    string.Concat("Our.Umbraco.DocTypeGridEditor.Web.Extensions.ContentTypeServiceExtensions.GetAliasById_", dataType.Key));
            }
        }
    }
}
