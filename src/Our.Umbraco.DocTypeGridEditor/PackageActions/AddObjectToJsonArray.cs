using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.PackageActions;
using Formatting = Newtonsoft.Json.Formatting;
using XmlHelper = Our.Umbraco.DocTypeGridEditor.Helpers.XmlHelper;

namespace Our.Umbraco.DocTypeGridEditor.PackageActions
{
    public class AddObjectToJsonArray : IPackageAction
    {
        public string Alias()
        {
            return "AddObjectToJsonArray";
        }
        
        private string GetKeyProperty(System.Xml.Linq.XElement xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "keyProperty");
        }

        private string GetSourceFileName(System.Xml.Linq.XElement xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "sourceFile");
        }

        private string GetTargetFileName(System.Xml.Linq.XElement xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "targetFile");
        }

        public bool Execute(string packageName, System.Xml.Linq.XElement xmlData)
        {
            try
            {
                var src = HttpContext.Current.Server.MapPath(GetSourceFileName(xmlData));
                var trg = HttpContext.Current.Server.MapPath(GetTargetFileName(xmlData));
                var propKey = GetKeyProperty(xmlData);

                if (File.Exists(src) == false || File.Exists(trg) == false)
                    return false;

                var srcJson = File.ReadAllText(src);
                var trgJson = File.ReadAllText(trg);

                if (string.IsNullOrWhiteSpace(srcJson))
                    return false;

                var srcObj = JsonConvert.DeserializeObject(srcJson) as JObject;
                var trgArr = JsonConvert.DeserializeObject(trgJson) as JArray ?? new JArray();

                if (srcObj == null)
                    return false;

                if (trgArr.Any(x => x[propKey] == srcObj[propKey]))
                    return false;

                trgArr.Add(srcObj);

                File.WriteAllText(trg, JsonConvert.SerializeObject(trgArr, Formatting.Indented), Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                Current.Logger.Error<AddObjectToJsonArray>("[DocTypeGridEditor] Error merging grid editor config.", ex);

                return false;
            }
        }

        public bool Undo(string packageName, System.Xml.Linq.XElement xmlData)
        {
            try
            {
                var src = HttpContext.Current.Server.MapPath(GetSourceFileName(xmlData));
                var trg = HttpContext.Current.Server.MapPath(GetTargetFileName(xmlData));
                var propKey = GetKeyProperty(xmlData);

                if (!File.Exists(src) || !File.Exists(trg))
                    return false;

                var srcJson = File.ReadAllText(src);
                var trgJson = File.ReadAllText(trg);

                if (string.IsNullOrWhiteSpace(srcJson))
                    return false;

                var srcObj = JsonConvert.DeserializeObject(srcJson) as JObject;
                var trgArr = JsonConvert.DeserializeObject(trgJson) as JArray ?? new JArray();

                if (srcObj == null)
                    return false;

                var idx = trgArr.FindIndex(x => x[propKey] == srcObj[propKey]);
                if (idx >= 0)
                {
                    trgArr.RemoveAt(idx);

                    File.WriteAllText(trg, JsonConvert.SerializeObject(trgArr, Formatting.Indented), Encoding.UTF8);
                }

                return true;
            }
            catch (Exception ex)
            {
                Current.Logger.Error<AddObjectToJsonArray>("[DocTypeGridEditor] Error unmerging grid editor config.", ex);

                return false;
            }
        }

        //public XmlNode SampleXml()
        //{
        //    var sample = string.Format(
        //        "<Action runat=\"install\" undo=\"true\" alias=\"{0}\" " +
        //            "keyProperty=\"alias\" " +
        //            "sourceFile=\"~/path/to/srcJson.js\" " +
        //            "targetFile=\"~/path/to/trgJson.js\" />",
        //        Alias());

        //    return helper.parseStringToXmlNode(sample);
        //}
    }
}