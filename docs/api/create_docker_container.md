# Creating a container

Testcontainers' generic container support offers the greatest flexibility and makes it easy to use virtually any container image in the context of a temporary test environment. To interact or exchange data with a container, Testcontainers provides `ContainerBuilder` to configure and create the resource.

## Configure container image

To specify the container image, use `WithImage(...)`.

The simplest overload accepts a `string`:

```csharp
_ = new ContainerBuilder()
  .WithImage("postgres:15.1");
```

For more advanced scenarios, `WithImage` also supports `IImage`, giving you more control over how the image is represented and its properties are resolved.

If you need to target a specific platform, the `DockerImage` implementation provides an overload that lets you explicitly set the platform, such as `linux/amd64`. By default, the container runtime uses the platform that matches the container host.

```csharp
_ = new ContainerBuilder()
  .WithImage(new DockerImage("postgres:15.1", new Platform("linux/amd64")));
```

!!!tip

    A specifier has the format `<os>|<arch>|<os>/<arch>[/<variant>]`. The user can provide either the operating system or the architecture or both. For more details, see [containerd/platforms](https://github.com/containerd/platforms).

## Configure container start

Both `ENTRYPOINT` and `CMD` allows you to configure an executable and parameters, that a container runs at the start. By default, a container will run whatever `ENTRYPOINT` or `CMD` is specified in the Docker container image. At least one of both configurations is necessary. The container builder implementation supports `WithEntrypoint(params string[])` and `WithCommand(params string[])` to set or override the executable. Ideally, the `ENTRYPOINT` should set the container's executable, whereas the `CMD` sets the default arguments for the `ENTRYPOINT`.

Instead of running the NGINX application, the following container configuration overrides the default start procedure of the image and just tests the NGINX configuration file.

```csharp
_ = new ContainerBuilder()
  .WithEntrypoint("nginx")
  .WithCommand("-t");
```

## Configure container app or service

Apps or services running inside a container are usually configured either with environment variables or configuration files. `WithEnvironment(string, string)` sets an environment variable, while `WithResourceMapping(string, string)` copies a file into a container before it starts. This covers common use cases among many .NET applications.

!!!tip

    The majority of builder methods are overloaded and have different parameters to set configurations.

To configure an ASP.NET Core application, either one or both mechanisms can be used.

```csharp
_ = new ContainerBuilder()
  .WithEnvironment("ASPNETCORE_URLS", "https://+")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/app/certificate.pfx")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "password")
  .WithResourceMapping("certificate.pfx", "/app/");
```

`WithBindMount(string, string)` is another option to provide access to directories or files. It mounts a host directory or file into the container. Note, this does not follow our best practices. Host paths differ between environments and may not be available on every system or Docker setup, e.g. CI.

## Copying directories or files to the container

Sometimes it is necessary to copy files into the container to configure the services running inside the container in advance, like the `appsettings.json` or an SSL certificate. The container builder API provides a member `WithResourceMapping(string, string)`, including several overloads to copy directories or individual files to a container's directory.

```csharp title="Copying a directory"
_ = new ContainerBuilder()
  .WithResourceMapping(new DirectoryInfo("."), "/app/");
```

```csharp title="Copying a file"
_ = new ContainerBuilder()
  // Copy 'appsettings.json' into the '/app' directory.
  .WithResourceMapping(new FileInfo("appsettings.json"), "/app/")
  // Copy 'appsettings.Container.json' to '/app/appsettings.Developer.json'.
  .WithResourceMapping(new FileInfo("appsettings.Container.json"), new FileInfo("/app/appsettings.Developer.json"));
```

Another overloaded member of the container builder API allows you to copy the contents of a byte array to a specific file path within the container. This can be useful when you already have the file content stored in memory or when you need to dynamically generate the file content before copying it.

```csharp title="Copying a byte array"
_ = new ContainerBuilder()
  .WithResourceMapping(Encoding.Default.GetBytes("{}"), "/app/appsettings.json");
```

### Specifying file ownership

When copying files into a container, you can specify the user ID (UID) and group ID (GID) to set the correct ownership of the copied files. This is particularly useful when the container runs as a non-root user or when specific file permissions are required for security or application functionality.

```csharp title="Copying a file with specific UID and GID"
_ = new ContainerBuilder()
  .WithResourceMapping(new DirectoryInfo("."), "/app/", uid: 1000, gid: 1000);
```

### Specifying file permission

When copying files into a container, you can specify the file mode to set the correct permissions for the copied files.

```csharp title="Copying a script with executable permissions"
_ = new ContainerBuilder()
  .WithResourceMapping(new DirectoryInfo("."), "/app/", fileMode: Unix.FileMode755);
```

The `Unix` class provides common permission configurations like `FileMode755` (read, write, execute for owner; read, execute for group and others). For individual permission combinations, you can use the `UnixFileModes` enumeration to create custom configurations.

### Copying files to a running container

The same UID, GID, and file mode arguments are also available when copying files to already running containers using the `IContainer.CopyAsync(...)` APIs.

## Reading files from the container

The `IContainer` interface offers a `ReadFileAsync(string, CancellationToken)` method to read files from the container. The implementation returns the read bytes. Either process the read bytes in-memory or persist them to the disk.

```csharp title="Reading a file"
var readBytes = await container.ReadFileAsync("/app/appsettings.json")
  .ConfigureAwait(false);

await File.WriteAllBytesAsync("appsettings.json", readBytes)
  .ConfigureAwait(false);
```

## Canceling a container start

Starting a container or creating a resource (such as a network or a volume) can be canceled by passing a `CancellationToken` to the member. The following example cancels the container start after one minute if it has not finished before.

```csharp title="Canceling container start after one minute"
using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
await _container.StartAsync(timeoutCts.Token)
  .ConfigureAwait(false);
```

## Getting log messages

Testcontainers for .NET provides two approaches for retrieving log messages from containers: `GetLogsAsync` and `WithOutputConsumer`. Each method serves different use cases for handling container logs.

The `GetLogsAsync` method is available through the `IContainer` interface. It allows you to fetch logs from a container for a specific time range or from the beginning until the present. This approach is useful for retrieving logs after a test has run, especially when troubleshooting issues or failures.

```csharp title="Getting all log messages"
var (stdout, stderr) = await _container.GetLogsAsync()
  .ConfigureAwait(false);
```

The `WithOutputConsumer` method is part of the `ContainerBuilder` class and is used to continuously forward container log messages to a specified output consumer. This approach provides real-time access to logs as the container runs.

```csharp title="Forwarding all log messages"
using IOutputConsumer outputConsumer = Consume.RedirectStdoutAndStderrToConsole();

_ = new ContainerBuilder()
  .WithOutputConsumer(outputConsumer);
```

The static class `Consume` offers pre-configured implementations of the `IOutputConsumer` interface for common use cases. If you need additional functionalities beyond those provided by the default implementations, you can create your own implementations of `IOutputConsumer`.

## Composing command arguments

Testcontainers for .NET provides the `WithCommand(ComposableEnumerable<string>)` API to give you flexible control over container command arguments. While currently used for container commands, the `ComposableEnumerable<T>` abstraction is designed to support other builder APIs in the future, allowing similar composition and override functionality.

Because our builders are immutable, this feature allows you to extend or override pre-configured configurations, such as those in Testcontainers [modules](../modules/index.md), without modifying the original builder.

`ComposableEnumerable<T>` lets you decide how new API arguments should be combined with existing ones. You can choose to append, overwrite, or apply other strategies based on your needs.

If a module applies default commands and you need to override or remove them entirely, you can do this e.g. by explicitly resetting the command list:

```csharp title="Resetting command arguments"
// Default PostgreSQL builder configuration:
//
// base.Init()
//   ...
//   .WithCommand("-c", "fsync=off")
//   .WithCommand("-c", "full_page_writes=off")
//   .WithCommand("-c", "synchronous_commit=off")
//   ...

var postgreSqlContainer = new PostgreSqlBuilder()
  .WithCommand(new OverwriteEnumerable<string>(Array.Empty<string>()))
  .Build();
```

Using `OverwriteEnumerable<string>(Array.Empty<string>())` removes all default command configurations. This is useful when you want full control over the PostgreSQL startup or when the default configurations do not match your requirements.

!!!tip

    You can create your own `ComposableEnumerable<T>` implementation to control exactly how configuration values are composed or modified.

## Examples

An NGINX container that binds the HTTP port to a random host port and hosts static content. The example connects to the web server and checks the HTTP status code.

```csharp
const ushort HttpPort = 80;

var nginxContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("nginx")
  .WithPortBinding(HttpPort, true)
  .Build();

await nginxContainer.StartAsync()
  .ConfigureAwait(false);

using var httpClient = new HttpClient();
httpClient.BaseAddress = new UriBuilder("http", nginxContainer.Hostname, nginxContainer.GetMappedPublicPort(HttpPort)).Uri;

var httpResponseMessage = await httpClient.GetAsync(string.Empty)
  .ConfigureAwait(false);

Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
```

This example creates and starts a container, that listens for incoming TCP connections and returns the magic number 42.

```csharp
const string MagicNumber = "42";

const ushort MagicNumberPort = 80;

var deepThoughtContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("alpine")
  .WithPortBinding(MagicNumberPort, true)
  .WithEnvironment("MAGIC_NUMBER", MagicNumber)
  .WithEntrypoint("/bin/sh", "-c")
  .WithCommand($"while true; do echo \"$MAGIC_NUMBER\" | nc -l -p {MagicNumberPort}; done")
  .Build();

await deepThoughtContainer.StartAsync()
  .ConfigureAwait(false);

using var magicNumberClient = new TcpClient(deepThoughtContainer.Hostname, deepThoughtContainer.GetMappedPublicPort(MagicNumberPort));
using var magicNumberReader = new StreamReader(magicNumberClient.GetStream());

var magicNumber = await magicNumberReader.ReadLineAsync()
  .ConfigureAwait(false);

Assert.Equal(MagicNumber, magicNumber);
```

!!!tip

    To avoid port conflicts, do not bind a fix host port. Instead, assign a random host port by using `WithPortBinding(80, true)` and retrieve it from the container instance by using `GetMappedPublicPort(80)`.

## Supported commands

| Builder method                | Description                                                                                                                                                                          |
|-------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DependsOn`                   | Sets the dependent resource to resolve and create or start before starting this container configuration.                                                                             |
| `WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.                                                                                                                                         |
| `WithAutoRemove`              | Will remove the stopped container automatically, similar to `--rm`.                                                                                                                  |
| `WithCleanUp`                 | Will remove the container automatically after all tests have been run.                                                                                                               |
| `WithLabel`                   | Applies metadata to the container e.g. `-l`, `--label "testcontainers=awesome"`.                                                                                                     |
| `WithImage`                   | Specifies an image for which to create the container.                                                                                                                                |
| `WithImagePullPolicy`         | Specifies an image pull policy to determine when an image is pulled e.g. <code>--pull "always" &vert; "missing" &vert; "never"</code>.                                               |
| `WithName`                    | Sets the container name e.g. `--name "testcontainers"`.                                                                                                                              |
| `WithHostname`                | Sets the container hostname e.g. `--hostname "testcontainers"`.                                                                                                                      |
| `WithMacAddress`              | Sets the container MAC address e.g. `--mac-address "00:80:41:ae:fd:7e"`.                                                                                                             |
| `WithWorkingDirectory`        | Specifies or overrides the `WORKDIR` for the instruction sets.                                                                                                                       |
| `WithEntrypoint`              | Specifies or overrides the `ENTRYPOINT` that runs the executable.                                                                                                                    |
| `WithCommand`                 | Specifies or overrides the `COMMAND` instruction provided in the Dockerfile.                                                                                                         |
| `WithEnvironment`             | Sets an environment variable in the container e.g. `-e`, `--env "MAGIC_NUMBER=42"`.                                                                                                  |
| `WithExposedPort`             | Exposes a port inside the container e.g. `--expose "80"`.                                                                                                                            |
| `WithPortBinding`             | Publishes a container port to the host e.g. `-p`, `--publish "80:80"`.                                                                                                               |
| `WithResourceMapping`         | Copies a file or any binary content into the created container even before it is started.                                                                                            |
| `WithBindMount`               | Binds a path of a file or directory into the container e.g. `-v`, `--volume ".:/tmp"`.                                                                                               |
| `WithVolumeMount`             | Mounts a managed volume into the container e.g. `--mount "type=volume,source=my-vol,destination=/tmp"`.                                                                              |
| `WithTmpfsMount`              | Mounts a temporary volume into the container e.g. `--mount "type=tmpfs,destination=/tmp"`.                                                                                           |
| `WithNetwork`                 | Assigns a network to the container e.g. `--network "bridge"`.                                                                                                                        |
| `WithNetworkAliases`          | Assigns a network-scoped aliases to the container e.g. `--network-alias "alias"`.                                                                                                    |
| `WithExtraHost`               | Adds a custom host-to-IP mapping to the container's `/etc/hosts` respectively `%WINDIR%\\system32\\drivers\\etc\\hosts` e.g. `--add-host "host.testcontainers.internal:172.17.0.2"`. |
| `WithPrivileged`              | Sets the `--privileged` flag.                                                                                                                                                        |
| `WithOutputConsumer`          | Redirects `stdout` and `stderr` to capture the container output.                                                                                                                     |
| `WithWaitStrategy`            | Sets the wait strategy to complete the container start and indicates when it is ready.                                                                                               |
| `WithStartupCallback`         | Sets the startup callback to invoke after the container start.                                                                                                                       |
| `WithCreateParameterModifier` | Allows low level modifications of the Docker container create parameter.                                                                                                             |

!!!tip

    Testcontainers for .NET detects your Docker host configuration. You do **not** have to set the Docker daemon socket.

!!!tip

    Testcontainers for .NET detects private Docker registry configurations and applies the credentials automatically to authenticate against registries.
