# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

Nothing yet.

## [0.3.0] - 2025-01-30

### Changed

- Moved data tables under a schema.

## [0.2.3] - 2025-01-29

### Added

- Read user by unique name.

## [0.2.2] - 2025-01-29

### Fixed

- Empty field value descriptions.

## [0.2.1] - 2025-01-29

### Fixed

- Number, String & RichText bounds validation and handling.

## [0.2.0] - 2025-01-28

### Added

- Content revision.
- Global (un)publishing.

### Changed

- Throw `ValidationException` when updating field definition (invariant content type, variant field definition).
- Handle `ContentFieldValueConflict` error in frontend.
- Use `FieldDefinition.Description` property.

### Fixed

- Number Input Steps.
- Published content search.
- CMS (frontend application) path.
- Do not include favicon with CMS assets.
- Required field values.
- New content locale publishing.
- Content (un)publishing after saving a draft.
- Lost changes on saves.

### Removed

- `SelectOption.IsPublished`

## [0.1.0] - 2025-01-26

Initial release, including basic account management, languages, 8 field types, content types & field definitions, and contents, invariant & translated, field values, indices and published contents.

[unreleased]: https://github.com/Logitar/CMS/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/Logitar/CMS/compare/v0.2.3...v0.3.0
[0.2.3]: https://github.com/Logitar/CMS/compare/v0.2.2...v0.2.3
[0.2.2]: https://github.com/Logitar/CMS/compare/v0.2.1...v0.2.2
[0.2.1]: https://github.com/Logitar/CMS/compare/v0.2.0...v0.2.1
[0.2.0]: https://github.com/Logitar/CMS/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/Logitar/CMS/releases/tag/v0.1.0
