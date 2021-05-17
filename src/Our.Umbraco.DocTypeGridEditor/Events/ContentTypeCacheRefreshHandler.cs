using Newtonsoft.Json;
using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;

namespace Our.Umbraco.DocTypeGridEditor.Events
{
    public class ContentTypeCacheRefreshHandler : INotificationHandler<ContentTypeCacheRefresherNotification>
    {
        private readonly AppCaches _appCaches;

        public ContentTypeCacheRefreshHandler(AppCaches appCaches)
        {
            _appCaches = appCaches;
        }

        public void Handle(ContentTypeCacheRefresherNotification notification)
        {
            if (notification.MessageType == MessageType.RefreshByJson)
            {
                var payload = JsonConvert.DeserializeAnonymousType((string)notification.MessageObject,
                    new[] { new { Alias = default(string) } });
                if (payload != null)
                {
                    foreach (var item in payload)
                    {
                        _appCaches.RuntimeCache.ClearByKey(
                            string.Concat(
                                "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypesByAlias_",
                                item.Alias));

                        // NOTE: Unsure how to get the doctype GUID, without hitting the database?
                        // So we end up clearing the entire cache for this key. [LK:2018-01-30]
                        _appCaches.RuntimeCache.ClearByKey(
                            "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetContentTypeAliasByGuid_");
                    }
                }
            }
        }
    }
}
