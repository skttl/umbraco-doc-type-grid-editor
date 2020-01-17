using Our.Umbraco.DocTypeGridEditor.ValueProcessing.Collections;
using Umbraco.Core.Composing;

namespace Our.Umbraco.DocTypeGridEditor.Extensions
{
    public static class CompositionExtensions
    {
        /// <summary>
        /// Used to modify the collection of Value Processors for Doc Type Grid Editor
        /// </summary>
        /// <param name="composition"></param>
        /// <returns></returns>
        public static DocTypeGridEditorValueProcessorsCollectionBuilder DocTypeGridEditorValueProcessors(this Composition composition)
            => composition.WithCollectionBuilder<DocTypeGridEditorValueProcessorsCollectionBuilder>();
    }
}