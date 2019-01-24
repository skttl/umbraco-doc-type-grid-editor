using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    internal class DetachedPublishedContent : PublishedContentWithKeyBase
    {
        private readonly Guid _key;
        private readonly string _name;
        private readonly PublishedContentType _contentType;
        private readonly IEnumerable<IPublishedProperty> _properties;
        private readonly bool _isPreviewing;
        private readonly IPublishedContent _containerNode;

        public DetachedPublishedContent(
            Guid key,
            string name,
            PublishedContentType contentType,
            IEnumerable<IPublishedProperty> properties,
            IPublishedContent containerNode = null,
            bool isPreviewing = false)
        {
            _key = key;
            _name = name;
            _contentType = contentType;
            _properties = properties;
            _containerNode = containerNode;
            _isPreviewing = isPreviewing;
        }

        public override Guid Key => _key;

        public override int Id => 0;

        public override string Name => _name;

        public override bool IsDraft => _isPreviewing;

        public override PublishedItemType ItemType => PublishedItemType.Content;

        public override PublishedContentType ContentType => _contentType;

        public override string DocumentTypeAlias => _contentType.Alias;

        public override int DocumentTypeId => _contentType.Id;

        public override ICollection<IPublishedProperty> Properties => _properties.ToArray();

        public override IPublishedProperty GetProperty(string alias) => _properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));

        public override IPublishedProperty GetProperty(string alias, bool recurse)
        {
            if (recurse)
                throw new NotSupportedException();

            return GetProperty(alias);
        }

        public override IPublishedContent Parent => null;

        public override IEnumerable<IPublishedContent> Children => Enumerable.Empty<IPublishedContent>();

        public override int TemplateId => 0;

        public override int SortOrder => 0;

        public override string UrlName => null;

        public override string WriterName => _containerNode?.WriterName;

        public override string CreatorName => _containerNode?.CreatorName;

        public override int WriterId => _containerNode?.WriterId ?? 0;

        public override int CreatorId => _containerNode?.CreatorId ?? 0;

        public override string Path => null;

        public override DateTime CreateDate => _containerNode?.CreateDate ?? DateTime.MinValue;

        public override DateTime UpdateDate => _containerNode?.UpdateDate ?? DateTime.MinValue;

        public override Guid Version => _containerNode?.Version ?? Guid.Empty;

        public override int Level => 0;
    }
}