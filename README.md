# Doc Type Grid Editor

[![Build status](https://img.shields.io/appveyor/ci/leekelleher/umbraco-doc-type-grid-editor.svg)](https://ci.appveyor.com/project/leekelleher/umbraco-doc-type-grid-editor)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor)
[![Chat on Gitter](https://img.shields.io/badge/gitter-join_chat-green.svg)](https://gitter.im/leekelleher/umbraco-nested-content)


A grid editor for Umbraco 7 that allows you to use Doc Types as a blue print for cell data.


## Getting Started

### Installation

> *Note:* Doc Type Grid Editor has been developed against **Umbraco v7.2.0** and will support that version and above.

Doc Type Grid Editor can be installed from either Our Umbraco package repository, or build manually from the source-code.

#### Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor) 

#### Manual build

If you prefer, you can compile  Doc Type Grid Editor yourself, you'll need:

* Visual Studio 2012 (or above)

To clone it locally click the "Clone in Windows" button above or run the following git commands.

	git clone https://github.com/leekelleher/umbraco-doc-type-grid-editor.git umbraco-doc-type-grid-editor
	cd umbraco-doc-type-grid-editor
	.\build.cmd

---

## Developers Guide

For details on how to use the Doc Type Grid Editor package, please refer to our [Developers Guide](docs/developers-guide.md) documentation.

A PDF download is also available: [Doc Type Grid Editor - Developers Guide v1.0.pdf](docs/assets/pdf/Doc-Type-Grid-Editor--Developers-Guide-v1.0.pdf)

---

## Known Issues

Please be aware that not all property-editors will work within Doc Type Grid Editor. The following property-editors are known to have compatibility issues:

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

* [Doc Type Grid Editor Forum](https://our.umbraco.org/projects/backoffice-extensions/doc-type-grid-editor/doc-type-grid-editor-feedback/) on Our Umbraco
* [Raise an issue](https://github.com/leekelleher/umbraco-doc-type-grid-editor/issues) on GitHub


## Dev Team

* [Matt Brailsford](https://github.com/mattbrailsford)
* [Lee Kelleher](https://github.com/leekelleher)

### Special thanks

* Thanks to [Jeavon Leopold](https://github.com/Jeavon) for being a rockstar and adding AppVeyor support.


## License

Copyright &copy; 2015 Umbrella Inc, Our Umbraco and [other contributors](https://github.com/leekelleher/umbraco-doc-type-grid-editor/graphs/contributors)

Licensed under the [MIT License](LICENSE.md)
