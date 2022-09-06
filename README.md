[![NuGet](https://img.shields.io/nuget/v/Testcontainers.svg)](https://www.nuget.org/packages/Testcontainers)
[![NuGet](https://img.shields.io/nuget/vpre/Testcontainers.svg)](https://www.nuget.org/packages/Testcontainers)
[![Continuous Integration](https://github.com/testcontainers/testcontainers-dotnet/actions/workflows/cicd.yml/badge.svg?branch=develop)](https://github.com/testcontainers/testcontainers-dotnet/actions/workflows/cicd.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=testcontainers_testcontainers-dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=testcontainers_testcontainers-dotnet)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=testcontainers_testcontainers-dotnet&metric=coverage)](https://sonarcloud.io/summary/new_code?id=testcontainers_testcontainers-dotnet)

# Testcontainers

Testcontainers is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions. The library is built on top of the .NET Docker remote API and provides a lightweight implementation to support your test environment in all circumstances.

Choose from existing pre-configured configurations and start containers within a second, to support and run your tests. Or create your own containers with Dockerfiles and run your tests immediately afterward.

Get in touch with the Testcontainers team and others, and join our [Slack workspace][slack-workspace].

## Supported operating systems

Testcontainers supports Windows, Linux, and macOS as host systems. Linux Docker containers are supported on all three operating systems.

Native Windows Docker containers are only supported on Windows. Windows requires the host operating system version to match the container operating system version. You'll find further information about Windows container version compatibility [here](https://docs.microsoft.com/en-us/virtualization/windowscontainers/deploy-containers/version-compatibility).

Keep in mind to enable the correct Docker engine on Windows host systems to match the container operating system. With Docker Desktop you can switch the engine either with the tray icon context menu or: `$env:ProgramFiles\Docker\Docker\DockerCli.exe -SwitchDaemon` or `-SwitchLinuxEngine`, `-SwitchWindowsEngine`.

## Supported commands

To configure a container, use the `TestcontainersBuilder<TestcontainersContainer>` builder, that provides:

- `WithImage` specifies an `IMAGE[:TAG]` to derive the container from.
- `WithWorkingDirectory` specifies and overrides the `WORKDIR` for the instruction sets.
- `WithEntrypoint` specifies and overrides the `ENTRYPOINT` that will run as an executable.
- `WithCommand` specifies and overrides the `COMMAND` instruction provided from the Dockerfile.
- `WithName` sets the container name e.g. `--name nginx`.
- `WithHostname` sets the container hostname e.g. `--hostname my-nginx`.
- `WithEnvironment` sets an environment variable in the container e.g. `-e, --env "test=containers"`.
- `WithLabel` applies metadata to the container e.g. `-l, --label testcontainers=awesome`.
- `WithExposedPort` exposes a port inside the container e.g. `--expose=80`.
- `WithPortBinding` publishes the container port to the host e.g. `-p, --publish 80:80`.
- `WithBindMount` binds a path of a file or directory into the container e.g. `-v, --volume .:/tmp`.
- `WithVolumeMount` mounts a managed volume into the container e.g. `--mount type=volume,source=.,destination=/tmp`.
- `WithTmpfsMount` mounts a temporary volume into the container e.g. `--mount type=tmpfs,destination=/tmp`.
- `WithNetwork` assigns a network to the container e.g. `--network="bridge"`.
- `WithNetworkAliases` assigns a network-scoped aliases to the container e.g. `--network-alias alias`
- `WithAutoRemove` will remove the stopped container automatically like `--rm`.
- `WithCleanUp` will remove the container automatically after all tests have been run (see [Resource Reaper](#resource-reaper)).
- `WithPrivileged` sets the `--privileged` flag.
- `WithDockerEndpoint` sets the Docker API endpoint e.g. `-H tcp://0.0.0.0:2376`.
- `WithRegistryAuthentication` basic authentication against a private Docker registry.
- `WithOutputConsumer` redirects `stdout` and `stderr` to capture the container output.
- `WithWaitStrategy` sets the wait strategy to complete the container start and indicates when it is ready.
- `WithCreateContainerParametersModifier` allows low level modifications of the Docker container create parameter.
- `WithStartupCallback` sets the startup callback to invoke after the container start.
- `WithResourceReaperSessionId` assigns a Resource Reaper session id to the container.

Use the additional builder for image (`ImageFromDockerfileBuilder`), network (`TestcontainersNetworkBuilder`) and volume (`TestcontainersVolumeBuilder`) to set up your individual test environment.

## Resource Reaper

Testcontainers assigns each Docker resource a Resource Reaper session id. After the tests are finished, [Ryuk][moby-ryuk] will take care of remaining Docker resources and removes them. You can change the Resource Reaper session and group Docker resources together with `WithResourceReaperSessionId`. Right now, only Linux containers are supported.

## Pre-configured containers

The pre-configured Testcontainers below are supported. Further examples can be found in [TestcontainersContainerTest][1] as well as in [database][2] or [message broker][3] tests.

- Apache CouchDB (couchdb:2.3.1)
- Azurite (mcr.microsoft.com/azure-storage/azurite:3.18.0)
- Couchbase (couchbase:6.5.1)
- Elasticsearch (elasticsearch:8.3.2)
- MariaDB (mariadb:10.8)
- Microsoft SQL Server (mcr.microsoft.com/mssql/server:2017-CU28-ubuntu-16.04)
- MongoDB (mongo:5.0.6)
- MySQL (mysql:8.0.28)
- Oracle Database (gvenzl/oracle-xe:21-slim)
- PostgreSQL (postgres:11.14)
- Redis (redis:5.0.14)
- Apache Kafka (confluentinc/cp-kafka:6.0.5)
- RabbitMQ (rabbitmq:3.7.28)

## Examples

Pulls `nginx`, creates a new container with port binding `80:80` and hits the default site.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage("nginx")
  .WithName("nginx")
  .WithPortBinding(80)
  .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80));

await using (var testcontainers = testcontainersBuilder.Build())
{
  await testcontainers.StartAsync();
  _ = WebRequest.Create("http://localhost:80");
}
```

Mounts the current directory as volume into the container and runs `hostname > /tmp/hostname` on startup.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage("nginx")
  .WithName("nginx")
  .WithBindMount(".", "/tmp")
  .WithEntrypoint("/bin/sh", "-c")
  .WithCommand("hostname > /tmp/hostname")
  .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists("/tmp/hostname"));

await using (var testcontainers = testcontainersBuilder.Build())
{
  await testcontainers.StartAsync();
}
```

Here is an example of a pre-configured container. In the example,  Testcontainers starts a PostgreSQL database in a [xUnit.net][xunit] test and executes a SQL query.

```csharp
public sealed class PostgreSqlTest : IAsyncLifetime
{
  private readonly TestcontainerDatabase testcontainers = new TestcontainersBuilder<PostgreSqlTestcontainer>()
    .WithDatabase(new PostgreSqlTestcontainerConfiguration
    {
      Database = "db",
      Username = "postgres",
      Password = "postgres",
    })
    .Build();

  [Fact]
  public void ExecuteCommand()
  {
    using (var connection = new NpgsqlConnection(this.testcontainers.ConnectionString))
    {
      using (var command = new NpgsqlCommand())
      {
        connection.Open();
        command.Connection = connection;
        command.CommandText = "SELECT 1";
        command.ExecuteReader();
      }
    }
  }

  public Task InitializeAsync()
  {
    return this.testcontainers.StartAsync();
  }

  public Task DisposeAsync()
  {
    return this.testcontainers.DisposeAsync().AsTask();
  }
}
```

The implementation of the pre-configured wait strategies can be chained together to support individual requirements for Testcontainers with different container platform operating systems.

```csharp
Wait.ForUnixContainer()
  .UntilPortIsAvailable(80)
  .UntilFileExists("/tmp/foo")
  .UntilFileExists("/tmp/bar")
  .UntilOperationIsSucceeded(() => true, 1);
```

## Logging

To enable and configure logging, set the static `TestcontainersSettings.Logger` property before test execution.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md)

## Authors

* **Andre Hofmeister** - *Initial work* - [HofmeisterAn](https://github.com/HofmeisterAn/)

## Thanks

Many thanks to [JetBrains](https://www.jetbrains.com/?from=testcontainers-dotnet) who provide an [Open Source License](https://www.jetbrains.com/community/opensource/) for this project :heart:.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

[1]: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Tests/Unit/Containers/Unix/TestcontainersContainerTest.cs
[2]: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Tests/Unit/Containers/Unix/Modules/Databases
[3]: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Tests/Unit/Containers/Unix/Modules/MessageBrokers
[slack-workspace]: https://slack.testcontainers.org/
[moby-ryuk]: https://github.com/testcontainers/moby-ryuk
[xunit]: https://xunit.net
