using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Models;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    internal class UnpublishedContent : PublishedContentWithKeyBase
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
            Mandate.ParameterNotNull(content, nameof(content));
            Mandate.ParameterNotNull(serviceContext, nameof(serviceContext));

            var userService = new Lazy<IUserService>(() => serviceContext.UserService);

            this.content = content;

            this.children = new Lazy<IEnumerable<IPublishedContent>>(() => this.content.Children().Select(x => new UnpublishedContent(x, serviceContext)).ToList());
            this.contentType = new Lazy<PublishedContentType>(() => PublishedContentType.Get(this.ItemType, this.DocumentTypeAlias));
            this.creatorName = new Lazy<string>(() => this.content.GetCreatorProfile(userService.Value).Name);
            this.parent = new Lazy<IPublishedContent>(() => new UnpublishedContent(this.content.Parent(), serviceContext));
            this.properties = new Lazy<Dictionary<string, IPublishedProperty>>(() => MapProperties(PropertyEditorResolver.Current, serviceContext));
            this.urlName = new Lazy<string>(() => this.content.Name.ToUrlSegment());
            this.writerName = new Lazy<string>(() => this.content.GetWriterProfile(userService.Value).Name);
        }

        public override Guid Key => this.content.Key;

        public override PublishedItemType ItemType => PublishedItemType.Content;

        public override int Id => this.content.Id;

        public override int TemplateId => this.content.Template?.Id ?? default(int);

        public override int SortOrder => this.content.SortOrder;

        public override string Name => this.content.Name;

        public override string UrlName => this.urlName.Value;

        public override string DocumentTypeAlias => this.content.ContentType?.Alias;

        public override int DocumentTypeId => this.content.ContentType?.Id ?? default(int);

        public override string WriterName => this.writerName.Value;

        public override string CreatorName => this.creatorName.Value;

        public override int WriterId => this.content.WriterId;

        public override int CreatorId => this.content.CreatorId;

        public override string Path => this.content.Path;

        public override DateTime CreateDate => this.content.CreateDate;

        public override DateTime UpdateDate => this.content.UpdateDate;

        public override Guid Version => this.content.Version;

        public override int Level => this.content.Level;

        public override bool IsDraft => true;

        public override IPublishedContent Parent => this.parent.Value;

        public override IEnumerable<IPublishedContent> Children => this.children.Value;

        public override PublishedContentType ContentType => this.contentType.Value;

        public override ICollection<IPublishedProperty> Properties => this.properties.Value.Values;

        public override IPublishedProperty GetProperty(string alias)
        {
            return this.properties.Value.TryGetValue(alias, out IPublishedProperty property) ? property : null;
        }

        private Dictionary<string, IPublishedProperty> MapProperties(PropertyEditorResolver resolver, ServiceContext services)
        {
            var contentType = this.contentType.Value;
            var properties = this.content.Properties;

            var items = new Dictionary<string, IPublishedProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var propertyType in contentType.PropertyTypes)
            {
                var property = properties.FirstOrDefault(x => x.Alias.InvariantEquals(propertyType.PropertyTypeAlias));
                var value = property?.Value;
                if (value != null)
                {
                    var propertyEditor = resolver.GetByAlias(propertyType.PropertyEditorAlias);
                    if (propertyEditor != null)
                    {
                        value = propertyEditor.ValueEditor.ConvertDbToString(property, property.PropertyType, services.DataTypeService);
                    }
                }

                items.Add(propertyType.PropertyTypeAlias, new UnpublishedProperty(propertyType, value));
            }

            return items;
        }
    }
}