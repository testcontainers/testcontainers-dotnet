# Creating an image

Testcontainers for .NET uses the builder design pattern to configure, create and delete Docker resources. It prepares and initializes your test environment and disposes of everything after your tests are finished — whether the tests are successful or not. To create a container image from a Dockerfile use `ImageFromDockerfileBuilder`.

## Examples

Builds and tags a new container image. The `Dockerfile` is located inside the `src` directory in the solution (`.sln`) directory.

```csharp
await new ImageFromDockerfileBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "src")
  .WithDockerfile("Dockerfile")
  .Build()
  .CreateAsync()
  .ConfigureAwait(false);
```

!!!tip

    The `Dockerfile` must be part of the build context, otherwise the build fails.

## Supported commands

| Builder method                | Description                                                                                                |
|-------------------------------|------------------------------------------------------------------------------------------------------------|
| `WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.                                                               |
| `WithCleanUp`                 | Will remove the image automatically after all tests have been run.                                         |
| `WithLabel`                   | Applies metadata to the image e.g. `-l`, `--label "testcontainers=awesome"`.                               |
| `WithName`                    | Sets the image name e.g. `-t`, `--tag "testcontainers:0.1.0"`.                                             |
| `WithDockerfile`              | Sets the name of the `Dockerfile`.                                                                         |
| `WithDockerfileDirectory`     | Sets the build context (directory path that contains the `Dockerfile`).                                    |
| `WithDeleteIfExists`          | Will remove the image if it already exists.                                                                |
| `WithBuildArgument`           | Sets build-time variables e.g `--build-arg "MAGIC_NUMBER=42"`.                                             |
| `WithCreateParameterModifier` | Allows low level modifications of the Docker image build parameter.                                        |

!!!tip

    Testcontainers for .NET detects your Docker host configuration. You do **not** have to set the Docker daemon socket.
