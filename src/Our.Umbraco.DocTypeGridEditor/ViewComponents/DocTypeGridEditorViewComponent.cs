using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.DocTypeGridEditor.ViewComponents
{
    public class DocTypeGridEditorViewComponent : ViewComponent
    {
        public virtual IViewComponentResult Invoke(dynamic model, string viewPath)
        {
            return View(viewPath, model);
        }
    }
}
