using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    /// <summary>
    /// A utility class used to help modify JSON data from the Umbraco property data table
    /// </summary>
    internal class JsonDbRow
    {
        public int Id { get; set; }

        public string RawData { get; set; }

        public JToken Data
        {
            get
            {
                return (JToken)JsonConvert.DeserializeObject(RawData);
            }
            set
            {
                RawData = JsonConvert.SerializeObject(value);
            }
        }
    }
}