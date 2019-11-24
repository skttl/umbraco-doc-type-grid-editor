using System;
using System.Linq;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
{
    internal class XmlHelper
    {
        public static string GetAttributeValueFromNode(System.Xml.Linq.XElement node, string attributeName)
        {
            return GetAttributeValueFromNode<string>(node, attributeName, string.Empty);
        }

        public static string GetAttributeValueFromNode(System.Xml.Linq.XElement node, string attributeName, string defaultValue)
        {
            return GetAttributeValueFromNode<string>(node, attributeName, defaultValue);
        }

        public static T GetAttributeValueFromNode<T>(System.Xml.Linq.XElement node, string attributeName, T defaultValue)
        {
            if (node != null && node.Attributes() != null && node.Attributes(attributeName) != null)
            {
                var result = node.Attributes(attributeName).FirstOrDefault();
                if (result != null && string.IsNullOrEmpty(result.Value))
                    return defaultValue;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            return defaultValue;
        }
    }
}