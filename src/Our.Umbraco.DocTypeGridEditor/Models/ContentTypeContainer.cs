using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    public class ContentTypeContainer
    {
        public PublishedContentType PublishedContentType { get; set; }
        public IContentType ContentType { get; set; }
    }
}
