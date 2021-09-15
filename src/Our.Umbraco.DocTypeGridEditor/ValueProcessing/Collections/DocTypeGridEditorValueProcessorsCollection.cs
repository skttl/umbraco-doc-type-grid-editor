using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;

namespace Our.Umbraco.DocTypeGridEditor.ValueProcessing.Collections
{
    /// <summary>
    /// Collection to hold references to Value Processors to be used with Doc Type Grid Editor. <see cref="IDocTypeGridEditorValueProcessor"/>
    /// </summary>
    public class DocTypeGridEditorValueProcessorsCollection : BuilderCollectionBase<IDocTypeGridEditorValueProcessor>
    {
        public DocTypeGridEditorValueProcessorsCollection(Func<IEnumerable<IDocTypeGridEditorValueProcessor>> items) : base(items)
        {

        }
    }
}
