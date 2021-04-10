using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor9.Models
{
    internal class UnpublishedContent : IPublishedContent
    {
        private readonly IContent content;

        private readonly Lazy<IEnumerable<IPublishedContent>> children;
        private readonly Lazy<IPublishedContentType> contentType;
        private readonly Lazy<IPublishedContent> parent;
        private readonly Lazy<Dictionary<string, IPublishedProperty>> properties;

        public UnpublishedContent(int id, ServiceContext serviceContext, PropertyEditorCollection propertyEditorCollection, IPublishedContentTypeFactory publishedContentTypeFactory)
            : this(serviceContext.ContentService.GetById(id), serviceContext, propertyEditorCollection, publishedContentTypeFactory)
        { }

        public UnpublishedContent(IContent content, ServiceContext serviceContext, PropertyEditorCollection propertyEditorCollection, IPublishedContentTypeFactory publishedContentTypeFactory)
            : base()
        {

            this.content = content;
            var contentType = serviceContext.ContentTypeService.Get(this.content.ContentType.Id);

            //this.children = new Lazy<IEnumerable<IPublishedContent>>(() => this.content.Children().Select(x => new UnpublishedContent(x, serviceContext)).ToList());
            this.contentType = new Lazy<IPublishedContentType>(() => publishedContentTypeFactory.CreateContentType(contentType));
            this.parent = new Lazy<IPublishedContent>(() => new UnpublishedContent(serviceContext.ContentService.GetById(this.content.ParentId), serviceContext, propertyEditorCollection, publishedContentTypeFactory));
            this.properties = new Lazy<Dictionary<string, IPublishedProperty>>(() => MapProperties(serviceContext, propertyEditorCollection));
            //this.urlName = new Lazy<string>(() => this.content.Name.ToUrlSegment());
            //this.writerName = new Lazy<string>(() => this.content.GetWriterProfile(userService.Value).Name);
        }

        public Guid Key => this.content.Key;

        IEnumerable<IPublishedProperty> IPublishedElement.Properties => Properties;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; }

        public PublishedItemType ItemType => PublishedItemType.Content;

        public string GetUrl(string culture = null)
        {
            return null;
        }

        public PublishedCultureInfo GetCulture(string culture = null)
        {
            return null;
        }

        bool IPublishedContent.IsDraft(string culture)
        {
            return true;
        }

        public bool IsPublished(string culture = null)
        {
            return false;
        }

        public int Id => this.content.Id;

        public string UrlSegment { get; }
        public int SortOrder => this.content.SortOrder;

        public string Name => this.content.Name;

        public int DocumentTypeId => this.content.ContentType?.Id ?? default(int);

        public int WriterId => this.content.WriterId;

        int? IPublishedContent.TemplateId => this.content.TemplateId;

        public int CreatorId => this.content.CreatorId;

        public string Path => this.content.Path;

        public DateTime CreateDate => this.content.CreateDate;

        public DateTime UpdateDate => this.content.UpdateDate;


        public int Level => this.content.Level;

        public bool IsDraft => true;

        public IPublishedContent Parent => this.parent.Value;

        public IEnumerable<IPublishedContent> Children => this.children.Value;

        public IPublishedContentType ContentType => this.contentType.Value;

        public ICollection<IPublishedProperty> Properties => this.properties.Value.Values;

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => this.children.Value;

        public IPublishedProperty GetProperty(string alias)
        {
            return this.properties.Value.TryGetValue(alias, out IPublishedProperty property) ? property : null;
        }

        private Dictionary<string, IPublishedProperty> MapProperties(ServiceContext services, PropertyEditorCollection dataEditors)
        {
            var contentType = this.contentType.Value;
            var properties = this.content.Properties;

            var items = new Dictionary<string, IPublishedProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var propertyType in contentType.PropertyTypes)
            {
                var property = properties.FirstOrDefault(x => x.Alias.InvariantEquals(propertyType.DataType.EditorAlias));
                var value = property?.GetValue();
                if (value != null)
                {
                    dataEditors.TryGet(propertyType.DataType.EditorAlias, out var editor);
                    if (editor != null)
                    {
                        value = editor.GetValueEditor().ConvertDbToString(property.PropertyType, value, services.DataTypeService);
                    }
                }

                items.Add(propertyType.DataType.EditorAlias, new UnpublishedProperty(propertyType, value));
            }

            return items;
        }
    }
}
