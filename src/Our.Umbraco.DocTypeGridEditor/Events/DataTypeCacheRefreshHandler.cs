using Newtonsoft.Json;
using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;

namespace Our.Umbraco.DocTypeGridEditor.Events
{
    public class DataTypeCacheRefreshHandler : INotificationHandler<DataTypeCacheRefresherNotification>
    {
        private readonly AppCaches _appCaches;

        public DataTypeCacheRefreshHandler(AppCaches appCaches)
        {
            _appCaches = appCaches;
        }

        public void Handle(DataTypeCacheRefresherNotification notification)
        {
            if (notification.MessageType == MessageType.RefreshByJson)
            {
                var payload = JsonConvert.DeserializeAnonymousType((string)notification.MessageObject,
                    new[] { new { Id = default(int), UniqueId = default(Guid) } });
                if (payload != null)
                {
                    foreach (var item in payload)
                    {
                        _appCaches.RuntimeCache.ClearByKey(
                            string.Concat(
                                "Our.Umbraco.DocTypeGridEditor.Web.Extensions.ContentTypeServiceExtensions.GetAliasById_",
                                item.UniqueId));

                        _appCaches.RuntimeCache.ClearByKey(
                            string.Concat(
                                "Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper.GetPreValuesCollectionByDataTypeId_",
                                item.Id));
                    }
                }
            }
        }
    }
}
