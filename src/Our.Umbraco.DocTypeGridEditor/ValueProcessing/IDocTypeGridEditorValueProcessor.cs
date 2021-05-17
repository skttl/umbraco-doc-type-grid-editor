using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.DocTypeGridEditor.ValueProcessing
{
    /// <summary>
    /// Abstracts a ValueProcessor that is used to modify a property value before it's sent to a Property Value Converter
    /// These are useful when the data format stored for DTGE is different from what the PVC expects ie. like Umbraco.Tags
    /// </summary>
    public interface IDocTypeGridEditorValueProcessor
    {
        /// <summary>
        /// Returns if this processor can handle a certain property editor based on the property editors alias.
        /// </summary>
        /// <param name="propertyEditorAlias">Contains the alias of the property editor ie. "Umbraco.Tags" or "Umbraco.Picker".</param>
        /// <returns></returns>
        bool IsProcessorFor(string propertyEditorAlias);

        /// <summary>
        /// Processes the value to de desired format/type. Most of the time this is to make sure that the object passed to the property value converters is
        /// of the correct type and in the correct format for the property value converter to handle.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object ProcessValue(object value);
    }
}
