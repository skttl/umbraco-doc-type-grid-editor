using Our.Umbraco.DocTypeGridEditor.Web.Controllers;
using System;
using Umbraco.Core;

namespace Our.Umbraco.DocTypeGridEditor.Composing
{
    public class Current
    {

        private static Type _defaultDocTypeGridEditorSurfaceControllerType;

        // internal - can only be accessed through Composition at compose time
        internal static Type DefaultDocTypeGridEditorSurfaceControllerType
        {
            get => _defaultDocTypeGridEditorSurfaceControllerType;
            set
            {
                if (value.IsOfGenericType(typeof(DocTypeGridEditorSurfaceController<>)) == false)
                    throw new InvalidOperationException($"The Type specified ({value}) is not of type {typeof(DocTypeGridEditorSurfaceController<>)}");
                _defaultDocTypeGridEditorSurfaceControllerType = value;
            }
        }
    }
}
