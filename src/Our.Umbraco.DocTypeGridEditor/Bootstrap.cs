using System.Web.Mvc;
using Our.Umbraco.DocTypeGridEditor.Web.Attributes;
using Our.Umbraco.DocTypeGridEditor.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Routing;

namespace Our.Umbraco.DocTypeGridEditor
{
    internal class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            GlobalFilters.Filters.Add(new DocTypeGridEditorPreviewAttribute());

            if (!DefaultDocTypeGridEditorSurfaceControllerResolver.HasCurrent)
            {
                DefaultDocTypeGridEditorSurfaceControllerResolver.Current = new DefaultDocTypeGridEditorSurfaceControllerResolver();
            }
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            DataTypeService.Saved += ExpireDataTypeCache;
            ContentTypeService.SavedContentType += ExpireContentTypeCache;
            PublishedContentRequest.Prepared += PublishedContentRequest_Prepared;
        }
        
        private void PublishedContentRequest_Prepared(object sender, EventArgs e)
        {
            var request = sender as PublishedContentRequest;
            // Check if it's a dtgePreview request and is set to redirect.
            // If so reset the redirect url to an empty string to stop the redirect happening in preview mode.
            if (request.Uri.Query.Contains("dtgePreview") && request.IsRedirect)
            {
                request.SetRedirect(string.Empty);
            }
        }

        private void ExpireDataTypeCache(IDataTypeService sender, SaveEventArgs<IDataTypeDefinition> e)
        {
            foreach (var dataType in e.SavedEntities)
            {
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(
                    string.Concat("Our.Umbraco.DocTypeGridEditor.Web.Extensions.ContentTypeServiceExtensions.GetAliasById_", dataType.Key));

                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(
                    string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_", dataType.Id));
            }
        }

        private void ExpireContentTypeCache(IContentTypeService sender, SaveEventArgs<IContentType> e)
        {
            foreach (var contentType in e.SavedEntities)
            {
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(
                    string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_", contentType.Alias));

                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(
                    string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_", contentType.Key));
            }
        }
    }
}
