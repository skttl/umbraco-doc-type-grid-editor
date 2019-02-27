using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor.Web
{
    public class PreviewModel : IPublishedElement
    {
        public IPublishedContent Page { get; set; }

        public IPublishedElement Item { get; set; }

        public string EditorAlias { get; set; }

        public string PreviewViewPath { get; set; }

        public string ViewPath { get; set; }

        #region IPublishedContent Implementation
        
        public IPublishedProperty GetProperty(string alias) => Item.GetProperty(alias);
        
        public PublishedContentType ContentType => Item.ContentType;

        public Guid Key => Item.Key;

        public IEnumerable<IPublishedProperty> Properties => Item.Properties;

        #endregion
    }
}