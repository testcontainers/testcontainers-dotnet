# Changelog

## [Unreleased]

### Added

- 481 Add builder access to the `CreateContainerParameters` instance (@Xitric)
- 483 Support custom resource reaper image via `TestcontainersSettings.ResourceReaperImage` (@vlaskal)
- 495 Add CHANGELOG.md
- 496 Support `~/.testcontainers.properties` custom configuration
- 500 Add trace output while building or pulling a Docker image (@michal-korniak)
- 501 Throw an exception when Docker image has not been built (@michal-korniak)
- 509 Check if the authentication provider can establish a Docker endpoint connection
- 510 Add `IImageFromDockerfileBuilder.WithBuildArgument` (@michal-korniak)
- 511 Remove temp Dockerfile archive after Docker build (@michal-korniak)
- 512 Throw ArgumentException if Docker image name contains uppercase characters (@michal-korniak)

### Removed

- 497 Remove `ResourceReaperDiagnostics`

### Fixed

- 449 Fix wrong mapped public host port
- 484 Fix unit tests that fail on Windows hosts (@vlaskal)
