using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.DocTypeGridEditor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.DocTypeGridEditor.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetDocTypeGridEditorSettings(this IServiceCollection services, Action<DocTypeGridEditorSettings> settings)
        {
            if (settings is not null)
            {
                services.Configure<DocTypeGridEditorSettings>(settings);
            }

            return services;
        }
    }
}
