using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor9.Web.Models
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

        public IPublishedContentType ContentType => Item.ContentType;

        public Guid Key => Item.Key;

        public IEnumerable<IPublishedProperty> Properties => Item.Properties;

        #endregion
    }
}
