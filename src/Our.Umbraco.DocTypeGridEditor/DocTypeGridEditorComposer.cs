using Our.Umbraco.DocTypeGridEditor.Events;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Our.Umbraco.DocTypeGridEditor.ValueProcessing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor
{
    public class DocTypeGridEditorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.DocTypeGridEditorValueProcessors().Append<UmbracoTagsValueProcessor>();
            builder.DataValueReferenceFactories().Append<DocTypeGridEditorDataValueReference>();
            builder.Services.AddUnique<DocTypeGridEditorHelper>();

            // Add Event handlers
            builder.AddNotificationHandler<DataTypeCacheRefresherNotification, DataTypeCacheRefreshHandler>();
            builder.AddNotificationHandler<ContentTypeCacheRefresherNotification, ContentTypeCacheRefreshHandler>();
        }
    }
}
