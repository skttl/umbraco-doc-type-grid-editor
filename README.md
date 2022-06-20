# Doc Type Grid Editor

[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.DocTypeGridEditor.svg)](https://www.nuget.org/packages/Our.Umbraco.DocTypeGridEditor)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor)

A grid editor for Umbraco that allows you to use Doc Types as a blue print for grid-cell data.

## Getting Started

### Installation

> _Note:_ Doc Type Grid Editor has been developed against Umbraco since v7 supports different versions of Umbraco:

DTGE is compatible with the following Umbraco versions:
Umbraco 7.1-7.5: DocTypeGridEditor 0.5.0
Umbraco 7.6-7.x: DocTypeGridEditor 0.6.0
Umbraco 8.1-8.5: DocTypeGridEditor 1.1.0
Umbraco 8.5-8.x: DocTypeGridEditor 1.2.7
Umbraco 9.0-9.x: DocTypeGridEditor 2.0.0
Umbraco 10.0-10.x: DocTypeGridEditor 10.0.0

Doc Type Grid Editor can be installed from either Our Umbraco package repository, or NuGet. From version 2.0.0 only NuGet can be used to install the package.

#### Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor)

#### NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.DocTypeGridEditor), you can run the following command from within Visual Studio:

    PM> Install-Package Our.Umbraco.DocTypeGridEditor

---

## Developers Guide

For details on how to use the Doc Type Grid Editor package, please refer to our documentation.

- [Doc Type Grid Editor - Developers Guide, v1.2.x](docs/developers-guide-v1.md)
- [Doc Type Grid Editor - Developers Guide, v2.x.x & v10.x.x](docs/developers-guide-v2.md)

---

## Known Issues

Please be aware that not all property-editors will work within Doc Type Grid Editor. The following Umbraco core property-editors are known to have compatibility issues:

- Image Cropper
- Macro Container
- Tags
- Upload

---

## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

- [Bug reports](CONTRIBUTING.md#bugs)
- [Feature requests](CONTRIBUTING.md#features)
- [Pull requests](CONTRIBUTING.md#pull-requests)

## Contact

Have a question?

- [Doc Type Grid Editor Forum](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor/doc-type-grid-editor-feedback/) on Our Umbraco
- [Raise an issue](https://github.com/skttl/umbraco-doc-type-grid-editor/issues) on GitHub

## Dev Team

- [Søren Kottal](https://github.com/skttl)

### Special thanks

- Thanks to [Matt Brailsford](https://github.com/mattbrailsford) and [Lee Kelleher](https://github.com/leekelleher) for building this great package.
- Thanks to [Jeavon Leopold](https://github.com/Jeavon) for being a rockstar and adding AppVeyor &amp; NuGet support.
- Thanks to [Dave Woestenborghs](https://github.com/dawoe) for helping solve showstopper issues.
- Thanks to [Arnold Visser](https://github.com/ArnoldV) and [Bjarne Fyrstenborg](https://github.com/bjarnef) for help with porting the package to Umbraco 8.

## License

Copyright &copy; 2019 Søren Kottal, Our Umbraco and [other contributors](https://github.com/skttl/umbraco-doc-type-grid-editor/graphs/contributors)

Copyright &copy; 2017 UMCO, Our Umbraco and [other contributors](https://github.com/skttl/umbraco-doc-type-grid-editor/graphs/contributors)

Copyright &copy; 2014 Umbrella Inc

Licensed under the [MIT License](LICENSE.md)
