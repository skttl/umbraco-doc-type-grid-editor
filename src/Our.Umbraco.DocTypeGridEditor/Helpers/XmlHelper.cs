using System;
using System.Xml;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
{
    internal class XmlHelper
    {
        public static string GetAttributeValueFromNode(XmlNode node, string attributeName)
        {
            return GetAttributeValueFromNode<string>(node, attributeName, string.Empty);
        }

        public static string GetAttributeValueFromNode(XmlNode node, string attributeName, string defaultValue)
        {
            return GetAttributeValueFromNode<string>(node, attributeName, defaultValue);
        }

        public static T GetAttributeValueFromNode<T>(XmlNode node, string attributeName, T defaultValue)
        {
            if (node != null && node.Attributes != null && node.Attributes[attributeName] != null)
            {
                var result = node.Attributes[attributeName].InnerText;
                if (string.IsNullOrEmpty(result))
                    return defaultValue;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            return defaultValue;
        }
    }
}