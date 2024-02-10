using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.BackOffice.ModelBinders;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Core.Dictionary;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Scoping;
using Microsoft.AspNetCore.Authorization;

namespace Our.Umbraco.DocTypeGridEditor.Controllers
{
    [PluginController("DocTypeGridEditorApi")]
    public class DocTypeGridEditorBlueprintApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentService _contentService;
        private ContentController _contentController;

        public DocTypeGridEditorBlueprintApiController(ICultureDictionary cultureDictionary, ILoggerFactory loggerFactory, IShortStringHelper shortStringHelper, IEventMessagesFactory eventMessages, ILocalizedTextService localizedTextService, PropertyEditorCollection propertyEditors, IContentService contentService, IUserService userService, IBackOfficeSecurityAccessor backofficeSecurityAccessor, IContentTypeService contentTypeService, IUmbracoMapper umbracoMapper, IPublishedUrlProvider publishedUrlProvider, IDomainService domainService, IDataTypeService dataTypeService, ILocalizationService localizationService, IFileService fileService, INotificationService notificationService, ActionCollection actionCollection, ISqlContext sqlContext, IJsonSerializer serializer, ICoreScopeProvider scopeProvider, IAuthorizationService authorizationService, IContentVersionService contentVersionService, ICultureImpactFactory cultureImpactFactory)
        {
            _contentController = new ContentController(cultureDictionary, loggerFactory, shortStringHelper, eventMessages, localizedTextService, propertyEditors, contentService, userService, backofficeSecurityAccessor, contentTypeService, umbracoMapper, publishedUrlProvider, domainService, dataTypeService, localizationService, fileService, notificationService, actionCollection, sqlContext, serializer, scopeProvider, authorizationService, contentVersionService, cultureImpactFactory);
        }

        [FileUploadCleanupFilter]
        [HttpPost]
        public async Task<ActionResult<ContentItemDisplay<ContentVariantDisplay>?>?> PostSaveBlueprint([ModelBinder(typeof(BlueprintItemBinder))] ContentItemSave contentItem)
        {
            return await _contentController.PostSaveBlueprint(contentItem);
        }
        [HttpDelete]
        [HttpPost]
        public IActionResult DeleteBlueprint(int id)
        {
            var found = _contentService.GetBlueprintById(id);

            if (found == null)
            {
                return BadRequest();
            }

            _contentService.DeleteBlueprint(found);

            return Ok();
        }
    }
}
