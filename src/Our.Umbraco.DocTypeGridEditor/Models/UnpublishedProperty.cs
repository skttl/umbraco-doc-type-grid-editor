using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    internal class UnpublishedProperty : IPublishedProperty
    {
        private readonly PublishedPropertyType propertyType;
        private readonly object dataValue;
        private readonly Lazy<bool> hasValue;
        private readonly Lazy<object> sourceValue;
        private readonly Lazy<object> objectValue;
        private readonly Lazy<object> xpathValue;

        public UnpublishedProperty(PublishedPropertyType propertyType, object value)
        {
            this.propertyType = propertyType;

            this.dataValue = value;
            this.hasValue = new Lazy<bool>(() => value != null && value.ToString().Trim().Length > 0);

            this.sourceValue = new Lazy<object>(() => this.propertyType.ConvertDataToSource(this.dataValue, true));
            this.objectValue = new Lazy<object>(() => this.propertyType.ConvertSourceToObject(this.sourceValue.Value, true));
            this.xpathValue = new Lazy<object>(() => this.propertyType.ConvertSourceToXPath(this.sourceValue.Value, true));
        }

        public string PropertyTypeAlias => this.propertyType.PropertyTypeAlias;

        public bool HasValue => this.hasValue.Value;

        public object DataValue => this.dataValue;

        public object Value => this.objectValue.Value;

        public object XPathValue => this.xpathValue.Value;
    }
}