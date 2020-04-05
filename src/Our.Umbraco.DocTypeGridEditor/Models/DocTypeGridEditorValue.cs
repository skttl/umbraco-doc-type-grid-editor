using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    public class DocTypeGridEditorValue
    {
        [JsonProperty("value")]
        public JObject Value { get; set; }
        [JsonProperty("dtgeContentTypeAlias")]
        public string ContentTypeAlias { get; set; }
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}
