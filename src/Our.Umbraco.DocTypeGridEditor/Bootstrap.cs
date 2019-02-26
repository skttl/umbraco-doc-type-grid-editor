using System;
using Newtonsoft.Json;
using Umbraco.Core.Composing;
using Umbraco.Core.Sync;
using Umbraco.Web.Cache;

namespace Our.Umbraco.DocTypeGridEditor
{

    public class Bootstrap : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // TODO: What is this for?
            //if (DefaultDocTypeGridEditorSurfaceControllerResolver.HasCurrent == false)
            //{
            //    DefaultDocTypeGridEditorSurfaceControllerResolver.Current = new DefaultDocTypeGridEditorSurfaceControllerResolver();
            //}

            DataTypeCacheRefresher.CacheUpdated += (sender, e) =>
            {
                if (e.MessageType == MessageType.RefreshByJson)
                {
                    var payload = JsonConvert.DeserializeAnonymousType((string) e.MessageObject,
                        new[] {new {Id = default(int), UniqueId = default(Guid)}});
                    if (payload != null)
                    {
                        foreach (var item in payload)
                        {
                            Current.AppCaches.RuntimeCache.ClearByKey(
                                string.Concat(
                                    "Our.Umbraco.DocTypeGridEditor.Web.Extensions.ContentTypeServiceExtensions.GetAliasById_",
                                    item.UniqueId));

                            Current.AppCaches.RuntimeCache.ClearByKey(
                                string.Concat(
                                    "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_",
                                    item.Id));
                        }
                    }
                }
            };

            ContentTypeCacheRefresher.CacheUpdated += (sender, e) =>
            {
                if (e.MessageType == MessageType.RefreshByJson)
                {
                    var payload = JsonConvert.DeserializeAnonymousType((string) e.MessageObject,
                        new[] {new {Alias = default(string)}});
                    if (payload != null)
                    {
                        foreach (var item in payload)
                        {
                            Current.AppCaches.RuntimeCache.ClearByKey(
                                string.Concat(
                                    "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_",
                                    item.Alias));

                            // NOTE: Unsure how to get the doctype GUID, without hitting the database?
                            // So we end up clearing the entire cache for this key. [LK:2018-01-30]
                            Current.AppCaches.RuntimeCache.ClearByKey(
                                "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_");
                        }
                    }
                }
            };
        }
    }
}