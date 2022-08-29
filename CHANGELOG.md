# Changelog

## [Unreleased]

### Added

- 421 Add Azurite module (@vlaskal)
- 504 Add Elasticsearch module (@chertby)
- 516 Add `ITestcontainersBuilder<TDockerContainer>.WithTmpfsMount` (@chrisbbe)
- 520 Add MariaDB module (@renemadsen)
- 528 Do not require the Docker host configuration (`DockerEndpointAuthConfig`) on `TestcontainersSettings` initialization
- 538 Support optional username and password in MongoDB connection string (@the-avid-engineer)
- 540 Add Docker registry authentication provider for `DOCKER_AUTH_CONFIG` environment variable (@vova-lantsov-dev)
- 541 Allow MsSqlTestcontainerConfiguration custom database names (@enginexon)

### Changed

- 571 Update `wnameless/oracle-xe-11g-r2` to `gvenzl/oracle-xe:21-slim`

### Fixed

- 525 Read ServerURL, Username and Secret field from CredsStore response to pull private Docker images

## [2.1.0]

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

- 431 Fix `System.InvalidOperationException : cannot hijack chunked or content length stream` (update `Docker.DotNet` dependency)
- 449 Fix wrong mapped public host port
- 484 Fix unit tests that fail on Windows hosts (@vlaskal)
- 507 Fix `ITestcontainersConfiguration` duplication, such as `DockerApiException : [...] Duplicate mount point [...]` (@alesandrino)
