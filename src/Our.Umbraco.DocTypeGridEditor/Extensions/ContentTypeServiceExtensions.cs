using System;
using Umbraco.Core.Cache;
using Umbraco.Web.Composing;
using Umbraco.Core.Services;

namespace Our.Umbraco.DocTypeGridEditor.Extensions
{
    internal static class ContentTypeServiceExtensions
    {
        public static string GetAliasByGuid(this IContentTypeService contentTypeService, Guid id)
        {
            var cache = Current.AppCaches.RequestCache;

            return cache.GetCacheItem<string>(
                string.Concat("Our.Umbraco.DocTypeGridEditor.Web.Extensions.ContentTypeServiceExtensions.GetAliasById_", id),
                () =>
                {
                    using (var scope = Current.ScopeProvider.CreateScope())
                        return scope.Database.ExecuteScalar<string>("SELECT [cmsContentType].[alias] FROM [cmsContentType] INNER JOIN [umbracoNode] ON [cmsContentType].[nodeId] = [umbracoNode].[id] WHERE [umbracoNode].[uniqueID] = @0", id);
                });
        }
    }
}
