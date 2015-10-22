# Doc Type Grid Editor

[![Build status](https://img.shields.io/appveyor/ci/leekelleher/umbraco-doc-type-grid-editor.svg)](https://ci.appveyor.com/project/leekelleher/umbraco-doc-type-grid-editor)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor)
[![Chat on Gitter](https://img.shields.io/badge/gitter-join_chat-green.svg)](https://gitter.im/leekelleher/umbraco-nested-content)


A grid editor for Umbraco 7 that allows you to use Doc Types as a blue print for cell data.


## Getting Started

### Installation

> *Note:* Nested Content has been developed against **Umbraco v7.1.4** and will support that version and above.

Nested Content can be installed from either Our Umbraco or NuGet package repositories, or build manually from the source-code:

#### Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [https://our.umbraco.org/projects/backoffice-extensions/nested-content](https://our.umbraco.org/projects/backoffice-extensions/nested-content) 

#### NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.NestedContent), you can run the following command from within Visual Studio:

	PM> Install-Package Our.Umbraco.NestedContent

We also have a [MyGet package repository](https://www.myget.org/gallery/umbraco-nested-content) - for bleeding-edge / development releases.

#### Manual build

If you prefer, you can compile  Nested Content yourself, you'll need:

* Visual Studio 2012 (or above)

To clone it locally click the "Clone in Windows" button above or run the following git commands.

	git clone https://github.com/leekelleher/umbraco-nested-content.git umbraco-nested-content
	cd umbraco-nested-content
	.\build.cmd

---

## Developers Guide

For details on how to use the Nested Content package, please refer to our [Developers Guide](docs/developers-guide.md) documentation.

A PDF download is also available: [Nested Content - Developers Guide v1.0.pdf](docs/assets/pdf/Nested-Content--Developers-Guide-v1.0.pdf)

---

## Known Issues

Please be aware that not all property-editors will work within Nested Content. The following property-editors are known to have compatibility issues:

* Upload (default Umbraco core)
* Image Cropper (default Umbraco core)
* Macro Container (default Umbraco core)

---

## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)


## Contact

Have a question?

* [Nested Content Forum](https://our.umbraco.org/projects/backoffice-extensions/nested-content/nested-content-feedback) on Our Umbraco
* [Raise an issue](https://github.com/leekelleher/umbraco-nested-content/issues) on GitHub


## Dev Team

* [Matt Brailsford](https://github.com/mattbrailsford)
* [Lee Kelleher](https://github.com/leekelleher)

### Special thanks

* Thanks to [Jeavon Leopold](https://github.com/Jeavon) for being a rockstar and adding AppVeyor support.


## License

Copyright &copy; 2015 Umbrella Inc, Our Umbraco and [other contributors](https://github.com/leekelleher/umbraco-nested-content/graphs/contributors)

Licensed under the [MIT License](LICENSE.md)
