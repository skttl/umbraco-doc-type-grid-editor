using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    internal class DetachedPublishedContent : PublishedContentModel
    {
        private readonly string _name;
        private readonly PublishedContentType _contentType;
        private readonly IEnumerable<IPublishedProperty> _properties;
        private readonly bool _isPreviewing;
        private readonly IPublishedContent _containerNode;

        public DetachedPublishedContent(IPublishedContent content) : base(content)
        {
            _name = content.Name;
            _contentType = content.ContentType;
            _properties = content.Properties;
            _isPreviewing = content.IsDraft();
            _containerNode = content.Parent;
        }

        public override int Id => 0;

        public override string Name => _name;

        public override bool IsDraft(string culture = null) => _isPreviewing;

        public override PublishedItemType ItemType => PublishedItemType.Content;

        public override PublishedContentType ContentType => _contentType;
        
        public override IPublishedProperty GetProperty(string alias) => _properties.FirstOrDefault(x => x.PropertyType.Alias.InvariantEquals(alias));

        public override IPublishedContent Parent => null;

        public override IEnumerable<IPublishedContent> Children => Enumerable.Empty<IPublishedContent>();

        public override int SortOrder => 0;

        public override string GetUrl(string culture = null) => null;

        public override string WriterName => _containerNode?.WriterName;

        public override string CreatorName => _containerNode?.CreatorName;

        public override int WriterId => _containerNode?.WriterId ?? 0;

        public override int CreatorId => _containerNode?.CreatorId ?? 0;

        public override string Path => null;

        public override DateTime CreateDate => _containerNode?.CreateDate ?? DateTime.MinValue;

        public override DateTime UpdateDate => _containerNode?.UpdateDate ?? DateTime.MinValue;

        public override int Level => 0;

    }
}