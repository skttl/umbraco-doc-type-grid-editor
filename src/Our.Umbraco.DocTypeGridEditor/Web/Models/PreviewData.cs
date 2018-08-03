using System.Runtime.Serialization;

namespace Our.Umbraco.DocTypeGridEditor.Web
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
    }
}