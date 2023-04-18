# Network communication

There are two common cases to set up a communication with containers or between containers. The first case is simple and does not require additional configurations. The host that runs the test (test process) resolves the container address from the running container with the `_container.Hostname` property.

!!! warning

    Do not use `localhost`, `127.0.0.1` or any other fix address to access the container. The address varies according to the environment.

To access apps or services running inside the container behind a certain port, expose and bind the container port to a random host port with `_builder.WithPortBinding(ushort, bool)` and resolve it with `_container.GetMappedPublicPort(ushort)`. It is recommended to use the `Hostname` property and `GetMappedPublicPort(ushort)` together when constructing addresses:

```csharp
_ = new UriBuilder("tcp", _container.Hostname, _container.GetMappedPublicPort(2375));
```

This is considered a best practice and prevents port collisions.

## Custom network

A more advanced case, places one or more containers on custom networks. The communication between those containers won't require exposing ports through the host anymore. To configure and create Docker networks use `NetworkBuilder`. The following example creates a network and assigns it to two containers. The second container establishes a network connection to Deep Thought by using its network alias and receives the magic number `42`.

```csharp
const string MagicNumber = "42";

const string MagicNumberHost = "deep-thought";

const ushort MagicNumberPort = 80;

var network = new NetworkBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .Build();

var deepThoughtContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("alpine")
  .WithEnvironment("MAGIC_NUMBER", MagicNumber)
  .WithEntrypoint("/bin/sh", "-c")
  .WithCommand($"while true; do echo \"$MAGIC_NUMBER\" | nc -l -p {MagicNumberPort}; done")
  .WithNetwork(network)
  .WithNetworkAliases(MagicNumberHost)
  .Build();

var ultimateQuestionContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("alpine")
  .WithEntrypoint("top")
  .WithNetwork(network)
  .Build();

await network.CreateAsync()
  .ConfigureAwait(false);

await Task.WhenAll(deepThoughtContainer.StartAsync(), ultimateQuestionContainer.StartAsync())
  .ConfigureAwait(false);

var execResult = await ultimateQuestionContainer.ExecAsync(new[] { "nc", MagicNumberHost, MagicNumberPort.ToString(CultureInfo.InvariantCulture) })
  .ConfigureAwait(false);

Assert.Equal(MagicNumber, execResult.Stdout.Trim());
```

## Exposing container ports to the host

It is common to connect to a container from your test process running on your test host. To bind and expose a container port, use the `WithPortBinding(ushort, true)` container builder member. To retrieve the actual port at runtime, use the container `GetMappedPublicPort(ushort)` member. Further information on network configurations is included in our [best practices](https://dotnet.testcontainers.org/api/best_practices/).

## Exposing host ports to the container

Sometimes containerized tests require access to services running on the test host. Setting up this connection can be very complex. Fortunately, Testcontainers simplifies the configuration to a bare minimum. It utilizing SSH and setting up an encrypted connection that forwards requests from a container to the test host.

In order to expose ports of services that are running on the test host to containers, we need to configure Testcontainers first. It is important to set up this configuration before configuring any container resources, otherwise, we are not able to forward traffic properly.

```csharp title="Exposing the host port 8080"
await TestcontainersSettings.ExposeHostPortsAsync(8080)
  .ConfigureAwait(false);
```

From the perspective of a container, the test host is available through the hostname `host.testcontainers.internal`. Ports are forwarded one-to-one.

```csharp title="Sending a GET request to the test host"
_ = await curlContainer.ExecAsync(new[] { "curl", "http://host.testcontainers.internal:8080" })
  .ConfigureAwait(false);
```

## Supported commands

| Builder method                | Description                                                                      |
|-------------------------------|----------------------------------------------------------------------------------|
| `WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.                                     |
| `WithCleanUp`                 | Will remove the network automatically after all tests have been run.             |
| `WithLabel`                   | Applies metadata to the network e.g. `-l`, `--label "testcontainers=awesome"`.   |
| `WithName`                    | Sets the network name e.g. `docker network create "testcontainers"`.             |
| `WithDriver`                  | Sets the network driver e.g. `-d`, `--driver "bridge"`                           |
| `WithOption`                  | Adds a driver specific option `-o`, `--opt "com.docker.network.driver.mtu=1350"` |
| `WithCreateParameterModifier` | Allows low level modifications of the Docker network create parameter.           |
