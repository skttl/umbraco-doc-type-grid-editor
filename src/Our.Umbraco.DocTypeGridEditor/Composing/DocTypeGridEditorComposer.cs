using Our.Umbraco.DocTypeGridEditor.Extensions;
using Our.Umbraco.DocTypeGridEditor.ValueProcessing;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Our.Umbraco.DocTypeGridEditor.Composing
{
    /// <summary>
    /// Composes defaults for Doc Type Grid Editor
    /// </summary>
    public class DocTypeGridEditorComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DocTypeGridEditorValueProcessors().Append<UmbracoTagsValueProcessor>();
            composition.DataValueReferenceFactories().Append<DocTypeGridEditorDataValueReference>();
        }
    }
}