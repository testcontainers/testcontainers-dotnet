# Creating a network

Testcontainers for .NET uses the builder design pattern to configure, create and delete Docker resources. It prepares and initializes your test environment and disposes of everything after your tests are finished — whether the tests are successful or not. To create a docker network use `TestcontainersNetworkBuilder`.

## Examples

Builds and create a new docker network.

```csharp
var network = new TestcontainersNetworkBuilder()
	.WithName(Guid.NewGuid().ToString("D"))
	.WithDriver(NetworkDriver.Bridge)
	.Build();

await network.CreateAsync();
```

## Supported commands

| Builder method                | Description                                                                                                          |
|-------------------------------|----------------------------------------------------------------------------------------------------------------------|
| `WithCleanUp`                 | Will remove the network automatically after all tests have been run.                                                 |
| `WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.                                                                         |
| `WithDriver`                  | Sets the driver of the Docker network e.g `-d`, `--driver=bridge`.                                                   |
| `WithLabel`                   | Applies metadata to the network e.g. `--label "testcontainers=awesome"`.                                             |
| `WithName`                    | Sets the network name e.g. `docker network create MyName`.                                                           |
| `WithOption`                  | Sets additional options of the network e.g. `-o`, `--opt "com.docker.network.bridge.host_binding_ipv4"="172.19.0.1"` |
| `WithResourceReaperSessionId` | Assigns a Resource Reaper session id to the network. The assigned Resource Reaper takes care of the cleanup.         |
