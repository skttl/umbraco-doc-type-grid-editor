  ____            _____                  ____      _     _ _____    _ _ _             
 |  _ \  ___   __|_   _|   _ _ __   ___ / ___|_ __(_) __| | ____|__| (_) |_ ___  _ __ 
 | | | |/ _ \ / __|| || | | | '_ \ / _ \ |  _| '__| |/ _` |  _| / _` | | __/ _ \| '__|
 | |_| | (_) | (__ | || |_| | |_) |  __/ |_| | |  | | (_| | |__| (_| | | || (_) | |   
 |____/ \___/ \___||_| \__, | .__/ \___|\____|_|  |_|\__,_|_____\__,_|_|\__\___/|_|   
                       |___/|_|                                                       

--------------------------------------------------------------------------------------

To enable DocTypeGridEditor you must add the following JSON snippet to "grid.editors.config.js" found in the Config folder.

{
    "name": "Doc Type",
    "alias": "docType",
    "view": "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html",
    "render": "/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml",
    "icon": "icon-item-arrangement",
    "config": {
        "allowedDocTypes": [],
        "nameTemplate": "",
        "enablePreview": true,
        "viewPath": "/Views/Partials/Grid/Editors/DocTypeGridEditor/",
        "previewViewPath": "/Views/Partials/Grid/Editors/DocTypeGridEditor/Previews/",
        "previewCssFilePath": "",
        "previewJsFilePath": ""
    }
}