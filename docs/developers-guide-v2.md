# Doc Type Grid Editor 2 - Developers Guide

### Contents

1. [Introduction](#introduction)
2. [Getting Set Up](#getting-set-up)
    1. [System Requirements](#system-requirements)
3. [Configuring The Doc Type Grid Editor](#configuring-the-doc-type-grid-editor)
4. [Hooking Up The Doc Type Grid Editor](#hooking-up-the-doc-type-grid-editor)
5. [Rendering a Doc Type Grid Editor](#rendering-a-doc-type-grid-editor)
    1. [Rendering Alternative Preview Content](#rendering-alternative-preview-content)
    2. [DocTypeGridEditorSurfaceController](#doctypegrideditorsurfacecontroller)
6. [Value Processors](#value-processors)
7. [Useful Links](#useful-links)

---

### Introduction

**Doc Type Grid Editor** is an advanced grid editor for the Umbraco grid, offering similar functionality as the macro grid editor but using the full power of the Doc Type editor and data types.

With the macro grid editor you are limited to only using the macro builder and thus the handful of parameter editors that are available. Of course you can create / config your own parameter editors, however this is cumbersome compared to how we can configure data types.

With the **Doc Type Grid Editor** then, we bridge that gap, allowing you to reuse doc type definitions as blue prints for complex data to be rendered in a grid cell.

---

### Getting Set Up

#### System Requirements

Before you get started, there are a number of things you will need:

1. .NET 5+
2. Umbraco 9.0.0+
3. The **Doc Type Grid Editor** package installed

---

### Configuring The Doc Type Grid Editor

The **Doc Type Grid Editor** is configured via package.manifest files located in `~/App_Plugins/*`. A default configuration is installed along with the package in `~/App_Plugins/DocTypeGridEditor/package.manifest`.  If you want to add your own editor configurations you can create your own package manifest file with the gridEditor config values (somewhere like `~/App_Plugins/DocTypeGridEditor.CustomEditors/package.manifest`).   For details on the configuration options, please see below.

#### Example

```javascript
[
    ...
    {
        "name": "Doc Type",
        "alias": "docType",
        "view": "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html",
        "render": "/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml",
        "icon": "icon-item-arrangement",
        "config": {
            "allowedDocTypes": [...],
            "nameTemplate": "",
            "enablePreview": true,
            "overlaySize": "medium",
            "viewPath": "/Views/Partials/Grid/Editors/DocTypeGridEditor/",
            "previewViewPath": "/Views/Partials/Grid/Editors/DocTypeGridEditor/Previews/",
            "previewCssFilePath": "",
            "previewJsFilePath": ""
        }
    },
    ...
]
```

For the main part, the root properties shouldn’t need to be modified, however the only properties that MUST not be changed are the **view** and **render** properties.

| Member | Type   | Description |
|--------|--------|-------------|
| Name   | String | The name of the grid editor as it appears in the grid editor prevalue editor / selector screen. |
| Alias  | String | A unique alias for this grid editor. |
| View   | String | The path to the **Doc Type Grid Editor** editor view. **MUST NOT BE CHANGED**. |
| Render | String | The path to the **Doc Type Grid Editor** render view. **MUST NOT BE CHANGED**. |
| Icon   | String | The icon class name to use for this grid editor (minus the '.') |
| Config | Object | Config options for this grid editor. |

The **Doc Type Grid Editor** supports 3 config options, all of which are optional.

| Member          | Type     | Description |
|-----------------|----------|-------------|
| AllowedDocTypes | String[] | An array of doc type aliases of which should be allowed to be selected in the grid editor. By default Strings are REGEX patterns to allow matching groups of doc types in a single entry. e.g. "widgetAlias" will match all doc types with an alias starting in "widgetAlias". By adding "$" to the end of the string you can stop this behaviour e.g. "widgetAlias$" will only match a doc type with an alias of "widgetAlias". However if a single doc type is matched, (aka **Single Doc Type Mode**), then doc type selection stage (in the DTGE panel) will be skipped. Note, your document type must be an Element type, in order to be usable in DTGE. |
| NameTemplate    | String   | Allows using any of the doctype's property values in the name/label: {{propertyAlias}} |
| EnablePreview   | Boolean  | Enables rendering a preview of the grid cell in the grid editor. |
| overlaySize     | String   | Define the size of the grid editor dialog. You can write `large`, `medium` or `small`. If no size is set, the grid editor dialog will be small. Note, the medium size requires a minimum Umbraco version of 8.3 |
| LargeDialog     | Boolean  | (obsolete, use overlaySize instead) Makes the editing dialog larger. Especially useful for grid editors with complex property editors. |
| size            | string   | (obsolete, use overlaySize instead) 
| ViewPath        | String   | Sets an alternative view path for where the **Doc Type Grid Editor** should look for views when rendering. Defaults to `~/Views/Partials/` |
| PreviewViewPath | String   | Sets an alternative view path for where the **Doc Type Grid Editor** should look for views when rendering previews in the backoffice |
| ShowDocTypeSelectAsGrid | Boolean | Makes the content type selection dialog render a grid, in stead of the default list with descriptions |

By default, a universal grid editor allowing all available element document types is added upon installation.

**Since Doc Type Grid Editor does not support culture variation, element document types allowing culture variations is not available for use in Doc Type Grid Editor.**

---

### Hooking Up The Doc Type Grid Editor

To hook up the **Doc Type Grid Editor**, within your grids prevalue, select the row configs you want to use the **Doc Type Grid Editor** in and for each cell config, check the **Doc Type** checkbox option to true. If you changed the name in the config, then select the item with the name you enter in the config. And, if you add your own editors either by package.manifest, or by editing grid.editors.config.js, you will need to select those too.

![Doc Type Grid Editor - Prevalue Editor](img/screenshot-01.png)

With the Doc Type Grid Editor enabled, from within your grid editor, you should now have a new option in the **Choose type of content** dialog.

![Doc Type Grid Editor - Insert Control](img/screenshot-02.png)

From there, simply click the **Doc Type** icon. If you have multiple document types matching the `AllowedDocTypes` setting in the selected grid editor (the default **Doc Type**, lets you pick between all available document types), you need to choose the document type you wish to render. 

![Doc Type Grid Editor - Select Doc Type](img/screenshot-03b.png)

If you have any Content Templates available for the selected document type, you need to choose which template to use for the content.

![Doc Type Grid Editor - Select Doc Type](img/screenshot-03.png)

Then you should be presented with a form for all the fields in your document type.

![Doc Type Grid Editor - Doc Type Fields](img/screenshot-04.png)

Fill in the fields and click Submit. You should then see the grid populated with a preview of your item. If you have disabled previews using the `EnablePreview` setting, an icon will be shown to represent your content block.

![Doc Type Grid Editor - Grid Preview](img/screenshot-05.png)

Make sure save / save &amp; publish the current page to persist your changes.

---

### Rendering a Doc Type Grid Editor

The **Doc Type Grid Editor** uses standard ASP.NET MVC partials as the rendering mechanism. By default it will look for partial files in the `ViewPath` setting of your grid editor, or in the default partial view location (for exmaple `~/Views/Partials`). The rendering mechanism looks for a file with a name that matches the document type alias. For example, for the default setup, if your document type alias is `TestDocType`, the Doc Type Grid Editor will look for the partial file `~/Views/Partials/Grid/Editors/DocTypeGridEditor/TestDocType.cshtml`.

To access the properties of your completed doc type, simply have your partial view inherit the standard `UmbracoViewPage` class, and you’ll be able to access them via the standard `Model` view property as a native `IPublishedElement` instance.

```
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<IPublishedElement>
<h3>@Model.Name</h3>
```

Because we treat your data as a standard `IPublishedElement` entity, that means you can use all the property value converters you are used to using.

```
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<IPublishedElement>
<h3>@Model.Name</h3>
@Model.Value("bodyText")
<a href="@(Model.Value<IPublishedContent>("link").Url)"> More</a>
```

Doc Type Grid Editor also supports ModelsBuilder, so you can simplify your views like this:
```
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<ContentModels.TestDoctype>
@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;
<h3>@Model.Name</h3>
@Model.BodyText
<a href="@(Model.Link.Url)"> More</a>
```

![Doc Type Grid Editor - Rendered Content](img/screenshot-06.png)


#### Rendering Alternative Preview Content

If your front end view is rather complex, you may decide that you want to feed the back office preview an alternative, less complex view. To do this, you can use the `PreviewViewPath` setting on your grid editor, and place a view named after your document type there. **Doc Type Grid Editor** will use the preview view file, when rendering previews in the backoffice.

If you don't want to have a seperate preview view file, you can add preview logic within your Razor view/partial. Check for a querystring parameter `dtgePreview` being set to "1" to detect being in preview mode to provide an alternative view.

```
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<IPublishedElement>
@if (Request.QueryString["dtgePreview"] == "1")
{
	// Render preview view
}
else
{
	// Render front end view
}
```

#### DocTypeGridEditorViewComponent
Behind the scenes, Doc Type Grid Editor uses a ViewComponent to render all items. You can let it use the default component for all your editors if you like, but you can also specify your own document type specific, or editor specific view component. Or specify your own default view component.

If you are not the type of developer that likes to put business logic in your views, then the ability to seperate logic from your view is a must. To render your grid editor item with a document type or editor specific view component, you need to create a view component, and give it a name in the format `{EditorAlias or Document Type Alias}DocTypeGridEditorViewComponent`. 

Note: The name of the viewcomponent is case sensitive, but DTGE is helpful enough to look for view components starting both with an uppercase letter and lowercase letter of your alias.

The view component must implement a method called Invoke taking a dynamic model parameter, and a viewPath parameter as a string. The method should return an IViewComponentResult. As an example, the default view component does this:

```cs
public IViewComponentResult Invoke(dynamic model, string viewPath)
{
    return View(viewPath, model);
}
```

#### Overriding the default view component
Doc Type Grid Editor comes with a default view component, that simply takes the model of the editor and sends it to the correct view. If you want to override this ViewComponent and replace it with your own, you can do it in your Startup.cs file, in the ConfigureServices method.

Add the following code, to configure DocTypeGridEditor:

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddUmbraco(_env, _config)
        .AddBackOffice()
        .AddWebsite()
        .AddComposers()
        .Build();

    services.SetDocTypeGridEditorSettings(c =>
    {
        c.DefaultDocTypeGridEditorViewComponent = typeof(MyDocTypeGridEditorViewComponent);
    });
}
```

---

### Value Processors
Since Doc Type Grid Editor stores the data for each property as a JSON-blob, we're not processing the values in the same way as Umbraco-core before storing it. This also means that the values that comes back and are passed into a Property Value Converter might be in of a type/format that that Property Value Convert can't handle. 

We've added something that we call "ValueProcessors" and these can be used to modify the raw property value before we send it to the property value converter. One example of where this is needed is the Tags-editor.

If you need to perform some processing of a value before it's sent to the property value converter you can add your own ValueProcessor.

```csharp
public class UmbracoColorPickerValueProcessor : IDocTypeGridEditorValueProcessor
{
    public bool IsProcessorFor(string propertyEditorAlias) 
          => propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.ColorPicker);

    public object ProcessValue(object value)
    {
        // Do something with the value
        return value;
    }
}
```

Then register this during composition withing IUserComposer,
```csharp
public class MyCustomDocTypeGridEditorComposer : IUserComposer
{
    public void Compose(Composition composition)
    {
        composition.DocTypeGridEditorValueProcessors()
                   .Append<UmbracoColorPickerValueProcessor>();
    }
}
```

Dot Type Grid editor ships with a ValueProcessor for the Umbraco-Tags property. 

**Note:** When using a Tag-editor inside a DTGE this would not create any relationship between the current node and that tag, if you need to tag a node you should use the Tags-editor as a property directly on the document type.

TODO: How to do this with IUmbracoBuilder?





### Useful Links

* [Source Code](https://github.com/skttl/umbraco-doc-type-grid-editor)
* [Our Umbraco Project Page](http://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor)
