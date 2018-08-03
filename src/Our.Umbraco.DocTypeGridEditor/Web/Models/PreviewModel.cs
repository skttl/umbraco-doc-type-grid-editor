using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor.Web
{
    public class PreviewModel : IPublishedContent
    {
        public IPublishedContent Page { get; set; }

        public IPublishedContent Item { get; set; }

        public string EditorAlias { get; set; }

        public string PreviewViewPath { get; set; }

        public string ViewPath { get; set; }

        #region IPublishedContent Implementation

        int IPublishedContent.GetIndex()
        {
            return Item.GetIndex();
        }

        IPublishedProperty IPublishedContent.GetProperty(string alias)
        {
            return Item.GetProperty(alias);
        }

        IPublishedProperty IPublishedContent.GetProperty(string alias, bool recurse)
        {
            return Item.GetProperty(alias, recurse);
        }

        IEnumerable<IPublishedContent> IPublishedContent.ContentSet => Item.ContentSet;

        PublishedContentType IPublishedContent.ContentType => Item.ContentType;

        int IPublishedContent.Id => Item.Id;

        int IPublishedContent.TemplateId => Item.TemplateId;

        int IPublishedContent.SortOrder => Item.SortOrder;

        string IPublishedContent.Name => Item.Name;

        string IPublishedContent.UrlName => Item.UrlName;

        string IPublishedContent.DocumentTypeAlias => Item.DocumentTypeAlias;

        int IPublishedContent.DocumentTypeId => Item.DocumentTypeId;

        string IPublishedContent.WriterName => Item.WriterName;

        string IPublishedContent.CreatorName => Item.CreatorName;

        int IPublishedContent.WriterId => Item.WriterId;

        int IPublishedContent.CreatorId => Item.CreatorId;

        string IPublishedContent.Path => Item.Path;

        DateTime IPublishedContent.CreateDate => Item.CreateDate;

        DateTime IPublishedContent.UpdateDate => Item.UpdateDate;

        Guid IPublishedContent.Version => Item.Version;

        int IPublishedContent.Level => Item.Level;

        string IPublishedContent.Url => Item.Url;

        PublishedItemType IPublishedContent.ItemType => Item.ItemType;

        bool IPublishedContent.IsDraft => Item.IsDraft;

        IPublishedContent IPublishedContent.Parent => Item.Parent;

        IEnumerable<IPublishedContent> IPublishedContent.Children => Item.Children;

        ICollection<IPublishedProperty> IPublishedContent.Properties => Item.Properties;

        object IPublishedContent.this[string alias] => Item[alias];

        #endregion
    }
}