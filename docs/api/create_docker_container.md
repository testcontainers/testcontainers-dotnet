# Creating a container

Testcontainers' generic container support offers the greatest flexibility and makes it easy to use virtually any container image in the context of a temporary test environment. To interact or exchange data with a container, Testcontainers provides  `TestcontainersBuilder<TDockerContainer>` to configure and create the resource.

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
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/app/certificate.crt")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "password")
  .WithResourceMapping("certificate.crt", "/app/certificate.crt");
```

`WithBindMount(string, string)` is another option to provide access to directories or files. It mounts a host directory or file into the container. Note, this does not follow our best practices. Host paths differ between environments and may not be available on every system or Docker setup, e.g. CI.

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
  .WithExposedPort(MagicNumberPort)
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

| Builder method                          | Description                                                                                                                            |
|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------|
| `WithDockerEndpoint`                    | Sets the Docker daemon socket to connect to.                                                                                           |
| `WithAutoRemove`                        | Will remove the stopped container automatically, similar to `--rm`.                                                                    |
| `WithCleanUp`                           | Will remove the container automatically after all tests have been run.                                                                 |
| `WithLabel`                             | Applies metadata to the container e.g. `-l`, `--label "testcontainers=awesome"`.                                                       |
| `WithImage`                             | Specifies an image for which to create the container.                                                                                  |
| `WithImagePullPolicy`                   | Specifies an image pull policy to determine when an image is pulled e.g. <code>--pull "always" &vert; "missing" &vert; "never"</code>. |
| `WithName`                              | Sets the container name e.g. `--name "testcontainers"`.                                                                                |
| `WithHostname`                          | Sets the container hostname e.g. `--hostname "testcontainers"`.                                                                        |
| `WithMacAddress`                        | Sets the container MAC address e.g. `--mac-address "00:80:41:ae:fd:7e"`.                                                               |
| `WithWorkingDirectory`                  | Specifies or overrides the `WORKDIR` for the instruction sets.                                                                         |
| `WithEntrypoint`                        | Specifies or overrides the `ENTRYPOINT` that runs the executable.                                                                      |
| `WithCommand`                           | Specifies or overrides the `COMMAND` instruction provided in the Dockerfile.                                                           |
| `WithEnvironment`                       | Sets an environment variable in the container e.g. `-e`, `--env "MAGIC_NUMBER=42"`.                                                    |
| `WithExposedPort`                       | Exposes a port inside the container e.g. `--expose "80"`.                                                                              |
| `WithPortBinding`                       | Publishes a container port to the host e.g. `-p`, `--publish "80:80"`.                                                                 |
| `WithResourceMapping`                   | Copies a file or any binary content into the created container even before it is started.                                              |
| `WithBindMount`                         | Binds a path of a file or directory into the container e.g. `-v`, `--volume ".:/tmp"`.                                                 |
| `WithVolumeMount`                       | Mounts a managed volume into the container e.g. `--mount "type=volume,source=my-vol,destination=/tmp"`.                                |
| `WithTmpfsMount`                        | Mounts a temporary volume into the container e.g. `--mount "type=tmpfs,destination=/tmp"`.                                             |
| `WithNetwork`                           | Assigns a network to the container e.g. `--network "bridge"`.                                                                          |
| `WithNetworkAliases`                    | Assigns a network-scoped aliases to the container e.g. `--network-alias "alias"`.                                                      |
| `WithPrivileged`                        | Sets the `--privileged` flag.                                                                                                          |
| `WithWaitStrategy`                      | Sets the wait strategy to complete the container start and indicates when it is ready.                                                 |
| `WithStartupCallback`                   | Sets the startup callback to invoke after the container start.                                                                         |
| `WithCreateParameterModifier`           | Allows low level modifications of the Docker container create parameter.                                                               |

!!!tip

    Testcontainers for .NET detects your Docker host configuration. You do **not** have to set the Docker daemon socket.

!!!tip

    Testcontainers for .NET detects private Docker registry configurations and applies the credentials automatically to authenticate against registries.
