using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using PropertyEditors = Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.DocTypeGridEditor.Models
{
    internal class DetachedPublishedProperty : IPublishedProperty
    {
        private readonly PublishedPropertyType _propertyType;
        private readonly object _rawValue;
        private readonly Lazy<object> _sourceValue;
        private readonly Lazy<object> _objectValue;
        private readonly Lazy<object> _xpathValue;
        private readonly bool _isPreview;

        public DetachedPublishedProperty(PublishedPropertyType propertyType, object value)
            : this(propertyType, value, false)
        { }

        public DetachedPublishedProperty(PublishedPropertyType propertyType, object value, bool isPreview)
        {
            _propertyType = propertyType;
            _isPreview = isPreview;

            _rawValue = value;

            _sourceValue = new Lazy<object>(() => _propertyType.ConvertSourceToInter(null, _rawValue, _isPreview));
            _objectValue = new Lazy<object>(() => _propertyType.ConvertInterToObject(null, PropertyEditors.PropertyCacheLevel.None, _sourceValue.Value, _isPreview));
            _xpathValue = new Lazy<object>(() => _propertyType.ConvertInterToXPath(null, PropertyEditors.PropertyCacheLevel.None, _sourceValue.Value, _isPreview));
        }

        public string PropertyTypeAlias
        {
            get
            {
                return _propertyType.DataType.EditorAlias;
            }
        }

        public bool HasValue
        {
            get { return DataValue != null && DataValue.ToString().Trim().Length > 0; }
        }

        public object DataValue
        {
            get { return _rawValue; }
        }

        public object Value
        {
            get { return _objectValue.Value; }
        }

        public object XPathValue
        {
            get { return _xpathValue.Value; }
        }

        bool IPublishedProperty.HasValue(string culture, string segment)
        {
            return HasValue;
        }

        public object GetSourceValue(string culture = null, string segment = null)
        {
            return DataValue;
        }

        public object GetValue(string culture = null, string segment = null)
        {
            return Value;
        }

        public object GetXPathValue(string culture = null, string segment = null)
        {
            return XPathValue;
        }

        public PublishedPropertyType PropertyType
        {
            get
            {
                return _propertyType;
            }
        }
        public string Alias { get; }
    }
}