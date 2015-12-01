using System;
using Our.Umbraco.DocTypeGridEditor.Web.Controllers;
using Umbraco.Core;
using Umbraco.Core.ObjectResolution;

namespace Our.Umbraco.DocTypeGridEditor.Web.Mvc
{
    /// <summary>
    /// A resolver used to resolve the default SurfaceController that is used to render any front-end
    /// Umbraco partial when using Nested Content.
    /// </summary>
    public class DefaultDocTypeGridEditorSurfaceControllerResolver : SingleObjectResolverBase<DefaultDocTypeGridEditorSurfaceControllerResolver, Type>
    {
        /// <summary>
        /// Constructor accepting the default SurfaceController
        /// </summary>
        /// <param name="value"></param>
        public DefaultDocTypeGridEditorSurfaceControllerResolver(Type value)
            : base(value)
        {
            ValidateType(value);
        }

        /// <summary>
        /// Sets the default SurfaceController type
        /// </summary>
        /// <param name="controllerType"></param>
        public void SetDefaultControllerType(Type controllerType)
        {
            ValidateType(controllerType);
            Value = controllerType;
        }

        /// <summary>
        /// Returns the Default SurfaceController type
        /// </summary>
        /// <returns></returns>
        public Type GetDefaultControllerType()
        {
            return Value;
        }

        /// <summary>
        /// Ensures that the type passed in is of type SurfaceController
        /// </summary>
        /// <param name="type"></param>
        private void ValidateType(Type type)
        {
            if (!type.IsOfGenericType(typeof(DocTypeGridEditorSurfaceController<>)))
            {
                throw new InvalidOperationException("The Type specified (" + type + ") is not of type " + typeof(DocTypeGridEditorSurfaceController<>));
            }
        }
    }
}
