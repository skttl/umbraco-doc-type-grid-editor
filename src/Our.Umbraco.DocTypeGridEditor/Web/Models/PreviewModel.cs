using System;
using System.Collections.Generic;
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
        
        public IPublishedProperty GetProperty(string alias) => Item.GetProperty(alias);
        
        public PublishedContentType ContentType => Item.ContentType;

        public Guid Key => Item.Key;

        public IEnumerable<IPublishedProperty> Properties => Item.Properties;

        public string GetUrl(string culture = null) => Item.GetUrl(culture);

        public PublishedCultureInfo GetCulture(string culture = null) => Item.GetCulture(culture);

        public bool IsDraft(string culture = null) => Item.IsDraft(culture);

        public bool IsPublished(string culture = null) => Item.IsPublished(culture);

        public int Id => Item.Id;

        public string Name => Item.Name;

        public string UrlSegment => Item.UrlSegment;

        public int SortOrder => Item.SortOrder;

        public int Level => Item.Level;

        public string Path => Item.Path;

        public int? TemplateId => Item.TemplateId;

        public int CreatorId => Item.CreatorId;

        public string CreatorName => Item.CreatorName;

        public DateTime CreateDate => Item.CreateDate;

        public int WriterId => Item.WriterId;

        public string WriterName => Item.WriterName;

        public DateTime UpdateDate => Item.UpdateDate;

        public string Url => Item.Url;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => Item.Cultures;

        public PublishedItemType ItemType => Item.ItemType;

        public IPublishedContent Parent => Item.Parent;

        public IEnumerable<IPublishedContent> Children => Item.Children;

        #endregion
    }
}