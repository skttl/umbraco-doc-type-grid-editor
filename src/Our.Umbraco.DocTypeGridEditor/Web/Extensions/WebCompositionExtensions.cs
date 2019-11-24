using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using DtgeCurrent = Our.Umbraco.DocTypeGridEditor.Composing.Current;

namespace Our.Umbraco.DocTypeGridEditor.Web.Extensions
{
    public static class WebCompositionExtensions
    {

        /// <summary>
        /// Sets the default controller for rendering DTGE views.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="composition">The composition.</param>
        /// <remarks>The controller type is registered to the container by the composition.</remarks>
        public static void SetDefaultDocTypeGridEditorSurfaceController<TController>(this Composition composition)
            => composition.SetDefaultDocTypeGridEditorSurfaceController(typeof(TController));

        /// <summary>
        /// Sets the default controller for rendering template views.
        /// </summary>
        /// <param name="composition">The composition.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <remarks>The controller type is registered to the container by the composition.</remarks>
        public static void SetDefaultDocTypeGridEditorSurfaceController(this Composition composition, Type controllerType)
        {
            composition.OnCreatingFactory["Our.Umbraco.DocTypeGridEditor.Web.DefaultDocTypeGridEditorSurfaceController"] = () =>
            {
                // no need to register: all IRenderMvcController are registered
                //composition.Register(controllerType, Lifetime.Request);
                DtgeCurrent.DefaultDocTypeGridEditorSurfaceControllerType = controllerType;
            };
        }
    }
}
