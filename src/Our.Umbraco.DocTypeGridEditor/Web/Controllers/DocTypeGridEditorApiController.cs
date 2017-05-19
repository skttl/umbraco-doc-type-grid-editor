using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web;

namespace Our.Umbraco.DocTypeGridEditor.Web.Controllers
{
    [PluginController("DocTypeGridEditorApi")]
    public class DocTypeGridEditorApiController : UmbracoAuthorizedJsonController
    {
        [System.Web.Http.HttpGet]
        public object GetContentTypeAliasByGuid([System.Web.Http.ModelBinding.ModelBinder] Guid guid)
        {
            return new
            {
                alias = Services.ContentTypeService.GetAliasByGuid(guid)
            };
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<object> GetContentTypes([System.Web.Http.ModelBinding.ModelBinder] string[] allowedContentTypes)
        {
            return Services.ContentTypeService.GetAllContentTypes()
                .Where(x => allowedContentTypes == null || allowedContentTypes.Length == 0 || allowedContentTypes.Any(y => Regex.IsMatch(x.Alias, y)))
                .OrderBy(x => x.SortOrder)
                .Select(x => new
                {
                    id = x.Id,
                    guid = x.Key,
                    name = x.Name,
                    alias = x.Alias,
                    icon = x.Icon
                });
        }

        [System.Web.Http.HttpGet]
        public object GetContentTypeIcon([System.Web.Http.ModelBinding.ModelBinder] string contentTypeAlias)
        {
            Guid docTypeGuid;
            if (Guid.TryParse(contentTypeAlias, out docTypeGuid))
                contentTypeAlias = Services.ContentTypeService.GetAliasByGuid(docTypeGuid);

            var contentType = Services.ContentTypeService.GetContentType(contentTypeAlias);
            return new
            {
                icon = contentType != null ? contentType.Icon : ""
            };
        }

        [System.Web.Http.HttpGet]
        public object GetDataTypePreValues(string dtdId)
        {
            Guid guidDtdId;
            int intDtdId;

            IDataTypeDefinition dtd;

            // Parse the ID
            if (int.TryParse(dtdId, out intDtdId))
            {
                // Do nothing, we just want the int ID
                dtd = Services.DataTypeService.GetDataTypeDefinitionById(intDtdId);
            }
            else if (Guid.TryParse(dtdId, out guidDtdId))
            {
                dtd = Services.DataTypeService.GetDataTypeDefinitionById(guidDtdId);
            }
            else
            {
                return null;
            }

            if (dtd == null)
                return null;

            // Convert to editor config
            var preValue = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);
            var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
            return propEditor.PreValueEditor.ConvertDbToEditor(propEditor.DefaultPreValues, preValue);
        }

        [System.Web.Http.HttpGet]
        public List<PropertyType> GetContentTypePropertyTypes(String alias)
        {
            var list = new List<PropertyType>();
            var doctype = UmbracoContext.Application.Services.ContentTypeService.GetContentType(alias);
            if (doctype != null)
            {
                list = doctype.PropertyTypes.ToList();
                if (doctype.ParentId != 0)
                {
                    list.AddRange(GetContentTypePropertyTypes(doctype.ParentId));
                }
            }
            return list;
        }
        private List<PropertyType> GetContentTypePropertyTypes(int id)
        {
            var list = new List<PropertyType>();
            var doctype = UmbracoContext.Application.Services.ContentTypeService.GetContentType(id);
            if (doctype != null)
            {
                list = doctype.PropertyTypes.ToList();
                if (doctype.ParentId != 0)
                {
                    list.AddRange(GetContentTypePropertyTypes(doctype.ParentId));
                }
            }
            return list;
        }
    }
}