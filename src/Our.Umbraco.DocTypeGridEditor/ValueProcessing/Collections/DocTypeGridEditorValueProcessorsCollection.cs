using System.Collections.Generic;
using Umbraco.Core.Composing;

namespace Our.Umbraco.DocTypeGridEditor.ValueProcessing.Collections
{
    /// <summary>
    /// Collection to hold references to Value Processors to be used with Doc Type Grid Editor. <see cref="IDocTypeGridEditorValueProcessor"/>
    /// </summary>
    public class DocTypeGridEditorValueProcessorsCollection : BuilderCollectionBase<IDocTypeGridEditorValueProcessor>
    {
        public DocTypeGridEditorValueProcessorsCollection(IEnumerable<IDocTypeGridEditorValueProcessor> items) : base(items)
        { }
    }
}