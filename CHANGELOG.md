# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 9.0.UNRELEASED
- Fixes bug in DataValueReference for media tracking, where custom DTGEs were not recognized. 4db5cdd

## 1.2.UNRELEASED
- Fixes bug in DataValueReference for media tracking, where custom DTGEs were not recognized. 4db5cdd

## 9.0.0
- Updated to Umbraco 9.0.0
- DocTypeGridEditorSurfaceController replaced by DocTypeGridEditorViewComponent **BREAKING**
- Finding moved cheese
- Changed DocTypeGridEditorHelper from a static class to being registered in the container for DI. **BREAKING**
- Adds default DocTypeGridEditorViewComponent used for all not hijacked rendering.
- Adds DocTypeGridEditorSettings object for configuring DocTypeGridEditor - currently only configures the default viewcomponent.

## 1.2.7 - 2020-03-28
- Fix: Null error when validation fails
- Fix: Resetting save button when validation fails

## 1.2.6 - 2020-12-07
- Fix: Error saving temp blueprint
- Fix: issue when previewing linked content items

## [1.2.5] - 2020-11-01
- Resetting saveButtonState when validation contains errors #221
- Handle null value when deserializing grid value in DataValueReference 50ba85aaeeb66ed305248d303d568e4698943094

## [1.2.4] - 2020-09-09
- Clear earlier serverside validation errors #217
- Null Pointer Exception on save when no grid layout selected #211
- Unsaved Changes dialog fires when hitting submit on grid editors #205

## [1.2.3] - 2020-06-05
- #199 2 Element Types in same Nested Content freeze the Add content button in the grid layout
- #202 Contenttemplates are left behind when validation is not succesfull
- #203 "activeVariant.language is null" - thanks @Matthew-Wise

## [1.2.2] - 2020-05-14
- Infinite loading when doc type has a content template #192 - thanks for reporting @OlePc
- GetPreviewMarkup null reference error when getting cultures #195 - thanks for reporting *and fixing* @Matthew-Wise

## [1.2.1] - 2020-04-27
- Disables notifications after each edit #190
- Moves intrusive styling in previews to css file #188
- Fixes JS error when no doctypes selected in multitype #186
- Fixes bug where wrong editor gets removed when cancelling #185
- Now works with custom backoffice URL #138

## [1.2.0] - 2020-04-12
- #173 Added ValueProcessors and added processor for Umbraco.Tags
- #176 Enables client side validation
- #182 Enables media tracking in 8.6

## [1.1.0] - 2019-12-11
- Changed the size attribute on grid editors to enable the new medium size in Umbraco 8.4 #167
- Adds class to the preview container, so it's easier to target with custom css 47c8f5d
- Check for culture variation when picking content types #154 f5453db c19a93f
- Added implementation of Alias property #160

## [1.0.0] - 2019-08-20
- Uses Infinite Editing (editorService) for editing DTGE content, instead of overlays.
- Adds option to set dialog (infinite editor) size from grid editor config.
- Adds Content Template aka Blueprints support for DTGE items.
- Updates default placeholder for DTGEs without previews enabled.
- Developers Guide updated
- New minimum Umbraco version requirement: 8.1.0 - Doc Type Grid Editor will not work in lower versions!

[unreleased]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.2.5...HEAD
[1.2.5]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.2.4...1.2.5
[1.2.4]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.2.3...1.2.4
[1.2.3]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.2.2...1.2.3
[1.2.2]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.2.1...1.2.2
[1.2.1]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.2.0...1.2.1
[1.2.0]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.1.0...1.2.0
[1.1.0]: https://github.com/skttl/umbraco-doc-type-grid-editor/compare/1.0.0...1.1.0
[1.0.0]: https://github.com/skttl/umbraco-doc-type-grid-editor/releases/tag/1.0.0
