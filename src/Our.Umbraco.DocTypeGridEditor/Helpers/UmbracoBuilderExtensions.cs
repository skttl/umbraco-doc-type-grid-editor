using Our.Umbraco.DocTypeGridEditor.ValueProcessing.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
{
    public static class UmbracoBuilderExtensions
    {
        /// <summary>
        /// Used to modify the collection of Value Processors for Doc Type Grid Editor
        /// </summary>
        /// <param name="composition"></param>
        /// <returns></returns>
        public static DocTypeGridEditorValueProcessorsCollectionBuilder DocTypeGridEditorValueProcessors(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<DocTypeGridEditorValueProcessorsCollectionBuilder>();
    }
}
