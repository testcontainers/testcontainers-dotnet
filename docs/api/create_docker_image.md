# Creating an image

Testcontainers for .NET uses the builder design pattern to configure, create and delete Docker resources. It prepares and initializes your test environment and disposes of everything after your tests are finished — whether the tests are successful or not. To create a container image from a Dockerfile use `ImageFromDockerfileBuilder`.

!!! warning

    `ImageFromDockerfileBuilder` builds the image through the Docker Engine API, which uses the legacy builder. BuildKit features are not supported through the Docker Engine API. As a result, Dockerfile instructions and options that depend on BuildKit cannot be used with it. For more details, see this [discussion](https://github.com/testcontainers/testcontainers-dotnet/discussions/1193#discussioncomment-10315903). Use [`BuildKitImageFromDockerfileBuilder`](#building-with-buildkit) to build such a Dockerfile.

## Examples

Builds and tags a new container image. The Dockerfile is located inside the solution (`.sln`) directory.

```csharp
var futureImage = new ImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithDockerfile("Dockerfile")
  .Build();

await futureImage.CreateAsync()
  .ConfigureAwait(false);
```

To build a Docker image with Testcontainers, it's important to understand the build context. Testcontainers needs three things:

1. **Docker build context**: The directory containing files Docker can use during the build
2. **Dockerfile name**: The name of the Dockerfile to use
3. **Dockerfile directory**: Where the Dockerfile is located

!!! tip

    The build context is optional. If you don't specify one, it defaults to the Dockerfile directory.

Testcontainers creates a tarball with all files and subdirectorys in the build context, incl. the Dockerfile. This tarball is sent to the Docker daemon to build the image. The build context acts as the root for all file operations in the Dockerfile, so all paths (like `COPY` commands) must be relative to it.

For example, if your project looks like this, the build context would be: `/Users/testcontainers/WeatherForecast/`.

    /
    └── Users/
        └── testcontainers/
            └── WeatherForecast/
                ├── src/
                │   ├── WeatherForecast.Entities/
                │   │   └── WeatherForecast.Entities.csproj
                │   └── WeatherForecast/
                │       └── WeatherForecast.csproj
                ├── tests/
                │   └── WeatherForecast.Tests/
                │       └── WeatherForecast.Tests.csproj
                ├── .dockerignore
                ├── Dockerfile
                └── WeatherForecast.sln

Testcontainers offers convenient features to detect common directories in .NET projects. The build configuration below resolves the directory containing the solution file by traversing up the directory tree from the executing assembly.

```csharp
_ = new ImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithDockerfile("Dockerfile");
```

As the tarball's content is based on `/Users/testcontainers/WeatherForecast/`, all paths inside the Dockerfile must be relative to this path. For example, Docker's `COPY` instruction copies all files inside the `WeatherForecast/` directory to the image.

!!! tip

    To improve the build time and to reduce the size of the image, it is recommended to include only necessary files. Exclude unnecessary files or directories such as `bin/`, `obj/` and `tests/` with the `.dockerignore` file.

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:6.0
ARG SLN_FILE_PATH="WeatherForecast.sln"
COPY . .
RUN dotnet restore $SLN_FILE_PATH
RUN dotnet publish $SLN_FILE_PATH --configuration Release --framework net6.0 --output app
ENTRYPOINT ["dotnet", "/app/WeatherForecast.dll"]
```

### Choosing a build context

You can use `WithContextDirectory(string)` to set a build context separate from your Dockerfile. This is useful when the Dockerfile is in one directory but the files you want to include are in another.

```csharp
_ = new ImageFromDockerfileBuilder()
  .WithContextDirectory("/path/to/build/context")
  .WithDockerfile("Dockerfile")
  .WithDockerfileDirectory("/path/to/dockerfile/directory");
```

## Delete multi-stage intermediate layers

A multi-stage Docker image build generates intermediate layers that serve as caches. Testcontainers' Resource Reaper is unable to automatically delete these layers after the test execution. The necessary label is not forwarded by the Docker image build. Testcontainers is unable to track the intermediate layers during the test. To delete the intermediate layers after the test execution, pass the Resource Reaper session to each stage.

The following Dockerfile assigns the `org.testcontainers.resource-reaper-session` label to each stage.

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env-1
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env-2
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
```

The `ImageFromDockerfileBuilder` provides a `WithBuildArgument(string, string)` member that passes a key-value to the Docker image build. We can leverage this mechanism to pass the appropriate Resource Reaper session to the build.

```csharp
_ = new ImageFromDockerfileBuilder()
  .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"));
```

## Building with BuildKit

`BuildKitImageFromDockerfileBuilder` builds the image with BuildKit (`docker buildx build`) instead of the Docker Engine API. Its configuration is the same as the one of `ImageFromDockerfileBuilder`, plus the members that only BuildKit supports. Use it for a Dockerfile that depends on BuildKit, such as one that contains a here-document, mounts a build secret, or selects a frontend with `# syntax=`.

```csharp
var futureImage = new BuildKitImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithDockerfile("Dockerfile")
  .Build();

await futureImage.CreateAsync()
  .ConfigureAwait(false);
```

The Docker CLI runs inside a container. Testcontainers copies the build context into that container, and mounts the Docker socket so the Docker CLI can reach the Docker daemon. You do not need a Docker CLI installation on the test host. The build itself runs in the BuildKit instance of the Docker daemon (the default `docker` Buildx driver), which is also where the build cache lives. The cache therefore outlives the container that starts the build, and is shared across builds the same way it is when you run `docker build` yourself.

The image is written to the image store of the Docker daemon (`--load`), so everything that follows the build behaves as it does with `ImageFromDockerfileBuilder`, including `WithImage(IImage)` and the Resource Reaper labels.

The Docker CLI image is configurable and pinned to a default. Pass a different one to run a specific Docker CLI and Buildx version. The image requires the Buildx plugin.

```csharp
_ = new BuildKitImageFromDockerfileBuilder("docker:29-cli");
```

!!! warning

    The Docker socket is bind-mounted into the Docker CLI container. The Docker daemon resolves the mount source, which is why a Docker daemon that is reached over TCP works too, as long as it listens on a Unix socket as well. A Docker daemon that does not provide a Unix socket at all, such as a Docker daemon that is reached over a Windows named pipe and runs Windows containers, cannot be used. Set `TestcontainersSettings.DockerSocketOverride` (or `TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE`) if the Docker socket is not at `/var/run/docker.sock`, or keep using `ImageFromDockerfileBuilder`.

### Build secrets

`WithSecret(string, string)` and `WithSecret(string, FileInfo)` pass a build secret to the build. The Dockerfile mounts it with `RUN --mount=type=secret,id=<id>`, which makes it available at `/run/secrets/<id>` for the duration of that instruction only. BuildKit does not add it to a layer of the built image.

```csharp
_ = new BuildKitImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithSecret("nuget", new FileInfo("/path/to/nuget.config"));
```

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY . .
RUN --mount=type=secret,id=nuget dotnet restore --configfile /run/secrets/nuget
```

Testcontainers copies the build secret into the Docker CLI container that runs the build. It is not part of the build context, and is not passed as a build argument or an environment variable. The container that runs the build is removed after the build, no matter whether the cleanup of the image is enabled or not.

### SSH agents

`WithSshAgent(string, params string[])` passes an SSH agent socket or private key to the build. The Dockerfile mounts it with `RUN --mount=type=ssh,id=<id>`. Use the id `default` for a mount that does not name an id. Each path is bind-mounted read-only into the Docker CLI container, keeping the path it has on the test host, so the paths must exist on the host that runs the Docker daemon. A path cannot contain a comma, which the Docker CLI uses to separate the paths of an SSH agent.

```csharp
_ = new BuildKitImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithSshAgent("default", Environment.GetEnvironmentVariable("SSH_AUTH_SOCK"));
```

### Platform

`WithPlatform(string)` builds the image for a platform other than the platform of the Docker host, for example `linux/arm64`. The build result is written to the image store of the Docker daemon, which takes a single platform only. Building for a foreign platform requires emulation, such as QEMU.

## Supported commands

| Builder method                | Description                                                                  |
|-------------------------------|------------------------------------------------------------------------------|
| `WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.                                 |
| `WithCleanUp`                 | Will remove the image automatically after all tests have been run.           |
| `WithLabel`                   | Applies metadata to the image e.g. `-l`, `--label "testcontainers=awesome"`. |
| `WithName`                    | Sets the image name e.g. `-t`, `--tag "testcontainers:0.1.0"`.               |
| `WithContextDirectory`        | Sets the Docker build context directory.                                     |
| `WithDockerfile`              | Sets the name of the `Dockerfile`.                                           |
| `WithDockerfileDirectory`     | Sets the directory path that contains the `Dockerfile`.                      |
| `WithImageBuildPolicy`        | Specifies an image build policy to determine when an image is built.         |
| `WithDeleteIfExists`          | Will remove the image if it already exists.                                  |
| `WithBuildArgument`           | Sets build-time variables e.g `--build-arg "MAGIC_NUMBER=42"`.               |
| `WithCreateParameterModifier` | Allows low level modifications of the Docker image build parameter.          |

`BuildKitImageFromDockerfileBuilder` supports the same members, and additionally:

| Builder method  | Description                                                              |
|-----------------|--------------------------------------------------------------------------|
| `WithSecret`    | Sets a build secret e.g. `--secret "id=aws,src=$HOME/.aws/credentials"`. |
| `WithSshAgent`  | Sets an SSH agent socket or private key e.g. `--ssh "default"`.          |
| `WithPlatform`  | Sets the platform to build the image for e.g. `--platform "linux/arm64"`.|

!!! tip

    Testcontainers for .NET detects your Docker host configuration. You do **not** have to set the Docker daemon socket.

!!! note

    `BuildKitImageFromDockerfileBuilder` translates the image build parameter (`WithCreateParameterModifier`) into Docker CLI arguments. The Dockerfile, the tags, the build arguments, the labels, the target and the platform are passed on, and so are `NoCache` (`--no-cache`), `Pull` (`--pull`), `NetworkMode` (`--network`), `ShmSize` (`--shm-size`), `ExtraHosts` (`--add-host`) and `CacheFrom` (`--cache-from`). A parameter that the Docker CLI does not provide an equivalent argument for, such as the resource limits of the legacy builder (`Memory`, `CPUShares`) or `Squash`, is logged as a warning instead of being applied.

## Known issues

- When building an image using Testcontainers for .NET and switching the user's context (`USER` statement) in a Dockerfile, the user won't automatically become the [owner](https://github.com/testcontainers/testcontainers-dotnet/issues/1171#issuecomment-2099197840) of the working directory, which seems to be the case when building the image from the CLI. If the running process requires write access to the working directory, it is necessary to set the permissions explicitly (the base image in this example already contains the user `app`):

   ```dockerfile
   FROM mcr.microsoft.com/dotnet/sdk:8.0
   WORKDIR /app
   RUN chown app:app .
   USER app
   ```
