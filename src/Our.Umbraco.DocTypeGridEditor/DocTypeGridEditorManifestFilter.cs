using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.DocTypeGridEditor
{
    public class DocTypeGridEditorManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest()
            {
                AllowPackageTelemetry = true,
                PackageName = "Doc Type Grid Editor",
                GridEditors = new[]
                {
                    new GridEditor()
                    {
                        Alias = "docType",
                        Name = "Doc Type",
                        View = "/App_Plugins/DocTypeGridEditor/Views/DocTypeGridEditor.html",
                        Render = "/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml",
                        Icon = "icon-science",
                        Config = new Dictionary<string, object>()
                        {
                            { "allowedDocTypes", Array.Empty<string>() },
                            { "nameTemplate", "" },
                            { "enablePreview", true },
                            { "viewPath", "/Views/Partials/grid/editors/DocTypeGridEditor/" },
                            { "previewViewPath", "/Views/Partials/grid/editors/DocTypeGridEditor/Previews/" },
                            { "previewCssFilePath", "" },
                            { "previewJsFilePath", "" }
                        }
                    }
                },
                Scripts = new[]
                {
                    "/App_Plugins/DocTypeGridEditor/Js/doctypegrideditor.resources.js",
                    "/App_Plugins/DocTypeGridEditor/Js/doctypegrideditor.services.js",
                    "/App_Plugins/DocTypeGridEditor/Js/doctypegrideditor.controllers.js",
                    "/App_Plugins/DocTypeGridEditor/Js/doctypegrideditor.directives.js"
                },
                Stylesheets = new[]
                {
                    "/App_Plugins/DocTypeGridEditor/Css/doctypegrideditor.css"
                }

            });
        }
    }
}
