using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.DocTypeGridEditor.Extensions;
using Umbraco.Core;
using Umbraco.Core.Composing.CompositionExtensions;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    internal class UnpublishedContent : IPublishedContent
    {
        private readonly IContent content;

        private readonly Lazy<IEnumerable<IPublishedContent>> children;
        private readonly Lazy<PublishedContentType> contentType;
        private readonly Lazy<string> creatorName;
        private readonly Lazy<IPublishedContent> parent;
        private readonly Lazy<Dictionary<string, IPublishedProperty>> properties;
        private readonly Lazy<string> urlName;
        private readonly Lazy<string> writerName;

        public UnpublishedContent(int id, ServiceContext serviceContext)
            : this(serviceContext.ContentService.GetById(id), serviceContext)
        { }

        public UnpublishedContent(IContent content, ServiceContext serviceContext)
            : base()
        {
            var userService = new Lazy<IUserService>(() => serviceContext.UserService);

            this.content = content;
            var contentType = Current.Services.ContentTypeService.Get(this.ContentType.Id);

            //this.children = new Lazy<IEnumerable<IPublishedContent>>(() => this.content.Children().Select(x => new UnpublishedContent(x, serviceContext)).ToList());
            this.contentType = new Lazy<PublishedContentType>(() => Current.PublishedContentTypeFactory.CreateContentType(contentType));
            this.creatorName = new Lazy<string>(() => this.content.GetCreatorProfile(userService.Value).Name);
            this.parent = new Lazy<IPublishedContent>(() => new UnpublishedContent(Current.Services.ContentService.GetById(this.content.ParentId), serviceContext));
            this.properties = new Lazy<Dictionary<string, IPublishedProperty>>(() => MapProperties(serviceContext));
            this.urlName = new Lazy<string>(() => this.content.Name.ToUrlSegment());
            this.writerName = new Lazy<string>(() => this.content.GetWriterProfile(userService.Value).Name);
        }

        public Guid Key => this.content.Key;

        IEnumerable<IPublishedProperty> IPublishedElement.Properties => Properties;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; }

        public PublishedItemType ItemType => PublishedItemType.Content;

        public string GetUrl(string culture = null)
        {
            throw new NotImplementedException();
        }

        public PublishedCultureInfo GetCulture(string culture = null)
        {
            throw new NotImplementedException();
        }

        bool IPublishedContent.IsDraft(string culture)
        {
            throw new NotImplementedException();
        }

        public bool IsPublished(string culture = null)
        {
            throw new NotImplementedException();
        }

        public int Id => this.content.Id;

        public string UrlSegment { get; }
        public int SortOrder => this.content.SortOrder;

        public string Name => this.content.Name;

        public string UrlName => this.urlName.Value;

        public string DocumentTypeAlias => this.content.ContentType?.Alias;

        public int DocumentTypeId => this.content.ContentType?.Id ?? default(int);

        public string WriterName => this.writerName.Value;

        public string CreatorName => this.creatorName.Value;

        public int WriterId => this.content.WriterId;

        int? IPublishedContent.TemplateId => this.content.TemplateId;

        public int CreatorId => this.content.CreatorId;

        public string Path => this.content.Path;

        public DateTime CreateDate => this.content.CreateDate;

        public DateTime UpdateDate => this.content.UpdateDate;
        public string Url { get; }


        public int Level => this.content.Level;

        public bool IsDraft => true;

        public IPublishedContent Parent => this.parent.Value;

        public IEnumerable<IPublishedContent> Children => this.children.Value;

        public PublishedContentType ContentType => this.contentType.Value;

        public ICollection<IPublishedProperty> Properties => this.properties.Value.Values;

        public IPublishedProperty GetProperty(string alias)
        {
            return this.properties.Value.TryGetValue(alias, out IPublishedProperty property) ? property : null;
        }

        private Dictionary<string, IPublishedProperty> MapProperties(ServiceContext services)
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
                    Current.PropertyEditors.TryGet(propertyType.DataType.EditorAlias, out var editor);
                    if (editor != null)
                    {
                        // TODO: FIXME!
                        value = editor.GetValueEditor().ConvertDbToString(property.PropertyType, value, services.DataTypeService);
                    }
                }

                items.Add(propertyType.DataType.EditorAlias, new UnpublishedProperty(propertyType, value));
            }

            return items;
        }
    }
}