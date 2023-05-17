# Creating an image

Testcontainers for .NET uses the builder design pattern to configure, create and delete Docker resources. It prepares and initializes your test environment and disposes of everything after your tests are finished — whether the tests are successful or not. To create a container image from a Dockerfile use `ImageFromDockerfileBuilder`.

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

!!!tip

    The Dockerfile must be part of the build context, otherwise the build fails.

It is essential to take into account and comprehend the build context to enable Testcontainers to build the Docker image. Testcontainers generates a tarball that contains all the files and subdirectories within the build context. The tarball is passed to the Docker daemon to build the image. The tarball serves as the new root of the Dockerfile's content. Therefore, all paths must be relative to the new root. If your app or service follows to the following project structure, the build context is `/Users/testcontainers/WeatherForecast/`.

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

!!!tip

    To improve the build time and to reduce the size of the image, it is recommended to include only necessary files. Exclude unnecessary files or directories such as `bin/`, `obj/` and `tests/` with the `.dockerignore` file.

```Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:6.0
ARG SLN_FILE_PATH="WeatherForecast.sln"
COPY . .
RUN dotnet restore $SLN_FILE_PATH
RUN dotnet publish $SLN_FILE_PATH --configuration Release --framework net6.0 --output app
ENTRYPOINT ["dotnet", "/app/WeatherForecast.dll"]
```

## Delete multi-stage intermediate layers

A multi-stage Docker image build generates intermediate layers that serve as caches. Testcontainers' Resource Reaper is unable to automatically delete these layers after the test execution. The necessary label is not forwarded by the Docker image build. Testcontainers is unable to track the intermediate layers during the test. To delete the intermediate layers after the test execution, pass the Resource Reaper session to each stage.

The following Dockerfile assigns the `org.testcontainers.resource-reaper-session` label to each stage.

```Dockerfile
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

## Supported commands

| Builder method                | Description                                                                  |
|-------------------------------|------------------------------------------------------------------------------|
| `WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.                                 |
| `WithCleanUp`                 | Will remove the image automatically after all tests have been run.           |
| `WithLabel`                   | Applies metadata to the image e.g. `-l`, `--label "testcontainers=awesome"`. |
| `WithName`                    | Sets the image name e.g. `-t`, `--tag "testcontainers:0.1.0"`.               |
| `WithDockerfile`              | Sets the name of the `Dockerfile`.                                           |
| `WithDockerfileDirectory`     | Sets the build context (directory path that contains the `Dockerfile`).      |
| `WithDeleteIfExists`          | Will remove the image if it already exists.                                  |
| `WithBuildArgument`           | Sets build-time variables e.g `--build-arg "MAGIC_NUMBER=42"`.               |
| `WithCreateParameterModifier` | Allows low level modifications of the Docker image build parameter.          |

!!!tip

    Testcontainers for .NET detects your Docker host configuration. You do **not** have to set the Docker daemon socket.
