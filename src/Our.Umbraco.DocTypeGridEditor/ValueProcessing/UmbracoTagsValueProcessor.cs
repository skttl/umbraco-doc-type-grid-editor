using Newtonsoft.Json;
using Umbraco.Core;

namespace Our.Umbraco.DocTypeGridEditor.ValueProcessing
{
    /// <summary>
    /// Processes the value stored in DTGE to match the string-json-format that the Property Value Converter expects.
    /// </summary>
    /// <remarks>
    /// When we're setting a value on a Umbraco.Core.Models.Property it will be converted into the type that this Property expects.
    /// In the case with Tags, there will be a .ToString() that converts the object passed into a string, something like System.Linq.Enumerable+WhereSelectEnumerableIterator`2[Newtonsoft.Json.Linq.JToken,System.String]
    /// This value would be passed to the Property Value Converter which would blow up. The processing below will safely convert the value to a valid json-string before it's passed
    /// to the Property Value Converter.
    /// </remarks>
    public class UmbracoTagsValueProcessor : IDocTypeGridEditorValueProcessor
    {
        public bool IsProcessorFor(string propertyEditorAlias) => propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.Tags);

        public object ProcessValue(object value)
        {
            if (value == null)
            {
                // When the value is null, we need to fake an empty array since this is
                // how Umbraco would store an empty "Umbraco.Tags"-property.
                return "[]";
            }

            // Returns a string-version of the JArray that DTGE would pass.
            return JsonConvert.SerializeObject(value);
        }
    }
}
