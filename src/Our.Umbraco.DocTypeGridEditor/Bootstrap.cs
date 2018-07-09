using System;
using Newtonsoft.Json;
using Our.Umbraco.DocTypeGridEditor.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Sync;
using Umbraco.Web.Cache;
using Umbraco.Web.Routing;

namespace Our.Umbraco.DocTypeGridEditor
{
    internal class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (DefaultDocTypeGridEditorSurfaceControllerResolver.HasCurrent == false)
            {
                DefaultDocTypeGridEditorSurfaceControllerResolver.Current = new DefaultDocTypeGridEditorSurfaceControllerResolver();
            }
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            DataTypeCacheRefresher.CacheUpdated += (sender, e) =>
            {
                if (e.MessageType == MessageType.RefreshByJson)
                {
                    var payload = JsonConvert.DeserializeAnonymousType((string)e.MessageObject, new[] { new { Id = default(int), UniqueId = default(Guid) } });
                    if (payload != null)
                    {
                        foreach (var item in payload)
                        {
                            applicationContext.ApplicationCache.RuntimeCache.ClearCacheItem(
                                string.Concat("Our.Umbraco.DocTypeGridEditor.Web.Extensions.ContentTypeServiceExtensions.GetAliasById_", item.UniqueId));

                            applicationContext.ApplicationCache.RuntimeCache.ClearCacheItem(
                                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_", item.Id));
                        }
                    }
                }
            };

            ContentTypeCacheRefresher.CacheUpdated += (sender, e) =>
            {
                if (e.MessageType == MessageType.RefreshByJson)
                {
                    var payload = JsonConvert.DeserializeAnonymousType((string)e.MessageObject, new[] { new { Alias = default(string) } });
                    if (payload != null)
                    {
                        foreach (var item in payload)
                        {
                            applicationContext.ApplicationCache.RuntimeCache.ClearCacheItem(
                                string.Concat("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_", item.Alias));

                            // NOTE: Unsure how to get the doctype GUID, without hitting the database?
                            // So we end up clearing the entire cache for this key. [LK:2018-01-30]
                            applicationContext.ApplicationCache.RuntimeCache.ClearCacheByKeySearch(
                                "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_");
                        }
                    }
                }
            };

            PublishedContentRequest.Prepared += (sender, e) =>
            {
                // Check if it's a dtgePreview request and is set to redirect.
                // If so reset the redirect url to an empty string to stop the redirect happening in preview mode.
                if (sender is PublishedContentRequest request && request.Uri.Query.InvariantContains("dtgePreview") && request.IsRedirect)
                {
                    request.SetRedirect(string.Empty);
                }
            };
        }
    }
}