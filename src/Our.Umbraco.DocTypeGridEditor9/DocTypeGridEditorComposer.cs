using Our.Umbraco.DocTypeGridEditor9.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor9
{
    public class DocTypeGridEditorComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<DocTypeGridEditorHelper>();
        }
    }
}
