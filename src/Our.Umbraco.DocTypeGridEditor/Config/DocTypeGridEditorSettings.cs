using Our.Umbraco.DocTypeGridEditor.ViewComponents;
using System;

namespace Our.Umbraco.DocTypeGridEditor.Config
{
    public class DocTypeGridEditorSettings
    {
        public Type DefaultDocTypeGridEditorViewComponent { get; set; } = typeof(DocTypeGridEditorViewComponent);
    }
}
