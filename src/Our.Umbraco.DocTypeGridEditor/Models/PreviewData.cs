using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    [DataContract]
    public class PreviewData
    {
        [DataMember(Name = "contentTypeAlias")]
        public string ContentTypeAlias { get; set; }

        [DataMember(Name = "editorAlias")]
        public string EditorAlias { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "previewViewPath")]
        public string PreviewViewPath { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "viewPath")]
        public string ViewPath { get; set; }

        [DataMember(Name = "culture")]
        public string Culture { get; set; }
    }
}
