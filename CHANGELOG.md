# Changelog

## [Unreleased]

### Added

- 640 Add `ITestcontainersBuilder<TDockerContainer>.WithResourceMapping` to copy files or or any binary contents into the created container even before it is started.
- 654 Add `ITestcontainersNetworkBuilder.WithOption` (@vlaskal)

### Changed

- 642 Expose container port bindings automatically
- 603 Add default logger that forwards messages to the console (does not support every test environment)

### Fixed

- 610 Trim traling slashes in Dockerfile directory path (otherwise, it cuts the first character of the relative path), Normalize paths to forward slashes
- 650 Update SharpZipLib to version 1.4.1 to prevent a deadlock in the Docker container image build
- 666 DockerImageNotFoundException when logged in with Docker Desktop instead of the CLI

## [2.2.0]

### Added

- 370 Add protected Docker daemon socket support (@vlaskal)
- 421 Add Azurite module (@vlaskal)
- 421 Add Cosmos DB Linux Emulator (@Yeseh, @ktjn)
- 504 Add Elasticsearch module (@chertby)
- 516 Add `ITestcontainersBuilder<TDockerContainer>.WithTmpfsMount` (@chrisbbe)
- 520 Add MariaDB module (@renemadsen)
- 528 Do not require the Docker host configuration (`DockerEndpointAuthConfig`) on `TestcontainersSettings` initialization
- 538 Support optional username and password in MongoDB connection string (@the-avid-engineer)
- 540 Add Docker registry authentication provider for `DOCKER_AUTH_CONFIG` environment variable (@vova-lantsov-dev)
- 541 Allow `MsSqlTestcontainerConfiguration` custom database names (@enginexon)
- 558 Support relative base directories other than the working directory with `WithDockerfileDirectory`
- 565 Add `ExecScriptAsync` (MongoDB Shell) to MongoDB module
- 579 Add Neo4j module (@kaiserbergin)
- 583 Add XML documentation to NuGet
- 592 Add LocalStack module (@bgener)
- 594 Add `IDockerContainer.GetLogs`
- 601 Add `ITestcontainersBuilder<TDockerContainer>.WithImagePullPolicy` (@BenasB)
- 616 Add `ITestcontainersBuilder<TDockerContainer>.WithMacAddress` (@seb1992)
- 618 Match `.dockerignore` entry `*` to all files and directories
- 626 Support MySQL root password configuration (@DanielHabenicht)

### Changed

- 571 Update `wnameless/oracle-xe-11g-r2` to `gvenzl/oracle-xe:21-slim`

### Fixed

- 525 Read ServerURL, Username and Secret field from CredsStore response to pull private Docker images
- 595 Implement `TestcontainersContainer.DisposeAsync` thread safe (rename `TestcontainersState` to `TestcontainersStates`)
- 604 Do not deny all files in the Docker image tarball when a `.dockerignore` entry ends with `/`
- 610 Do not deny all files in the Docker image tarball when a `.dockerignore` entry ends with `/*`
- 632 Execute local database scripts (inside the container) against `localhost`
- 634 JsonReaderException in Docker.DotNet with Docker Desktop 4.13.0 (https://github.com/dotnet/Docker.DotNet/issues/595)

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
