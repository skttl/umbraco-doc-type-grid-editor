using System.Net.Http.Formatting;
using Umbraco.Core.Models;

namespace Our.Umbraco.DocTypeGridEditor.Web
{
    public class PreviewModel
    {
        public IPublishedContent Page { get; set; }

        public FormDataCollection Values { get; set; }
    }
}