using Newtonsoft.Json;
using Our.Umbraco.DocTypeGridEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.DocTypeGridEditor.ValueProcessing
{
    public class DocTypeGridEditorDataValueReference : IDataValueReferenceFactory, IDataValueReference
    {
        private readonly Lazy<Dictionary<string, IContentType>> _contentTypes;

        public IDataValueReference GetDataValueReference() => this;

        public bool IsForEditor(IDataEditor dataEditor) => dataEditor.Alias.InvariantEquals(Constants.PropertyEditors.Aliases.Grid);


        public DocTypeGridEditorDataValueReference()
        {
            _contentTypes = new Lazy<Dictionary<string, IContentType>>(() => Current.Services.ContentTypeService.GetAll().ToDictionary(c => c.Alias));
        }

        public IEnumerable<UmbracoEntityReference> GetReferences(object value)
        {
            var result = new List<UmbracoEntityReference>();
            var _propertyEditors = Current.PropertyEditors;
            var rawJson = value == null ? string.Empty : value is string str ? str : value.ToString();

            if(rawJson.IsNullOrWhiteSpace()) return result;

            DeserializeGridValue(rawJson, out var dtgeValues);

            foreach (var control in dtgeValues)
            {
                if (_contentTypes.Value.TryGetValue(control.ContentTypeAlias, out var contentType))
                {
                    var propertyTypes = contentType.CompositionPropertyTypes.ToDictionary(x => x.Alias, x => x);
                    var properties = control.Value.Properties();

                    foreach (var property in properties)
                    {
                        if (propertyTypes.TryGetValue(property.Name, out var propertyType))
                        {
                            if (_propertyEditors.TryGet(propertyType.PropertyEditorAlias, out var propertyEditor))
                            {
                                if (propertyEditor.GetValueEditor() is IDataValueReference reference)
                                {
                                    var propertyValue = property.Value.ToString();
                                    var refs = reference.GetReferences(propertyValue);
                                    result.AddRange(refs);
                                }
                            }
                        }
                    }
                }
            }
            return result;

        }

        internal GridValue DeserializeGridValue(string rawJson, out IEnumerable<DocTypeGridEditorValue> dtgeValues)
        {
            var grid = JsonConvert.DeserializeObject<GridValue>(rawJson);

            if (grid != null)
            {
                // Find all controls that uses DTGE editor
                var controls = grid.Sections.SelectMany(x => x.Rows.SelectMany(r => r.Areas).SelectMany(a => a.Controls)).ToArray();
                dtgeValues = controls.Where(x => x.Editor.Alias.ToLowerInvariant() == "doctype").Select(x => x.Value.ToObject<DocTypeGridEditorValue>());
            }
            else
            {
                dtgeValues = Enumerable.Empty<DocTypeGridEditorValue>();
            }

            return grid;
        }
    }
}