using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Newtonsoft.Json;
using Our.Umbraco.DocTypeGridEditor.Models;

namespace Our.Umbraco.DocTypeGridEditor.ValueProcessing
{
    public class DocTypeGridEditorDataValueReference : IDataValueReferenceFactory, IDataValueReference
    {
        private readonly Lazy<Dictionary<string, IContentType>> _contentTypes;
        private readonly Lazy<PropertyEditorCollection> _dataEditors;

        public IDataValueReference GetDataValueReference() => this;

        public bool IsForEditor(IDataEditor dataEditor) => dataEditor.Alias.InvariantEquals(Constants.PropertyEditors.Aliases.Grid);


        public DocTypeGridEditorDataValueReference(Lazy<IContentTypeService> contentTypeService, Lazy<PropertyEditorCollection> dataEditors)
        {
            _contentTypes = new Lazy<Dictionary<string, IContentType>>(() => contentTypeService.Value.GetAll().ToDictionary(c => c.Alias));
            _dataEditors = dataEditors;
        }

        public IEnumerable<UmbracoEntityReference> GetReferences(object value)
        {
            var result = new List<UmbracoEntityReference>();
            var rawJson = value == null ? string.Empty : value is string str ? str : value.ToString();

            if (rawJson.IsNullOrWhiteSpace()) return result;

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
                            if (_dataEditors.Value.TryGet(propertyType.PropertyEditorAlias, out var propertyEditor))
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
                dtgeValues = controls.Where(x => x.Value["dtgeContentTypeAlias"] != null).Select(x => x.Value.ToObject<DocTypeGridEditorValue>());
            }
            else
            {
                dtgeValues = Enumerable.Empty<DocTypeGridEditorValue>();
            }

            return grid;
        }
    }
}
