using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

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

            // TODO: FIXME!
            //_sourceValue = new Lazy<object>(() => _propertyType.ConvertDataToSource(_rawValue, _isPreview));
            //_objectValue = new Lazy<object>(() => _propertyType.ConvertSourceToObject(_sourceValue.Value, _isPreview));
            //_xpathValue = new Lazy<object>(() => _propertyType.ConvertSourceToXPath(_sourceValue.Value, _isPreview));
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
            throw new NotImplementedException();
        }

        public object GetSourceValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        public object GetXPathValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        public PublishedPropertyType PropertyType { get; }
        public string Alias { get; }
    }
}