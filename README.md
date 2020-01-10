[![NuGet](https://img.shields.io/nuget/v/DotNet.Testcontainers.svg)](https://www.nuget.org/packages/DotNet.Testcontainers)
[![Build Status](https://dev.azure.com/HofmeisterAn/GitHub-Testcontainers/_apis/build/status/Build?branchName=refs/heads/develop)](https://dev.azure.com/HofmeisterAn/GitHub-Testcontainers/_build/latest?definitionId=15&branchName=refs/heads/develop)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=dotnet-testcontainers&metric=alert_status)](https://sonarcloud.io/dashboard?id=dotnet-testcontainers)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=dotnet-testcontainers&metric=coverage)](https://sonarcloud.io/dashboard?id=dotnet-testcontainers)

# .NET Testcontainers

.NET Testcontainers is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions. The library is built on top of the .NET Docker remote API and provides a lightweight implementation to support your test environment in all circumstances.

Choose from existing pre-configured configurations and start containers within a second, to support and run your tests. Or create your own containers with Dockerfiles and run your tests immediately afterward.

## Supported operating systems

.NET Testcontainers supports Windows, Linux, and macOS as host systems. Linux Docker containers are supported on all three operating systems.

Native Windows Docker containers are only supported on Windows. Windows requires the host operating system version to match the container operating system version. You'll find further information about Windows container version compatibility [here](https://docs.microsoft.com/en-us/virtualization/windowscontainers/deploy-containers/version-compatibility).

Keep in mind to enable the correct Docker engine on Windows host systems to match the container operating system. With Docker CE you can switch the engine with: `$env:ProgramFiles\Docker\Docker\DockerCli.exe -SwitchDaemon` or `-SwitchLinuxEngine`, `-SwitchWindowsEngine`.

## Supported commands

- `WithImage` specifies an `IMAGE[:TAG]` to derive the container from.
- `WithWorkingDirectory` specifies and overrides the `WORKDIR` for the instruction sets.
- `WithEntrypoint` specifies and overrides the `ENTRYPOINT` that will run as an executable.
- `WithCommand` specifies and overrides the `COMMAND` instruction provided from the Dockerfile.
- `WithName` sets the container name e. g. `--name nginx`.
- `WithEnvironment` sets an environment variable in the container e. g. `-e, --env "test=containers"`.
- `WithLabel` applies metadata to a container e. g. `-l, --label dotnet.testcontainers=awesome`.
- `WithExposedPort` exposes a port inside the container e. g. `--expose=80`.
- `WithPortBinding` publishes a container port to the host e. g. `-p, --publish 80:80`.
- `WithMount` mounts a volume into the container e. g. `-v, --volume .:/tmp`.
- `WithCleanUp` removes a stopped container automatically.
- `WithDockerEndpoint` sets the Docker API endpoint e. g. `-H tcp://0.0.0.0:2376`.
- `WithOutputConsumer` redirects `stdout` and `stderr` to capture the Testcontainer output.
- `WithWaitStrategy` sets the wait strategy to complete the Testcontainer start and indicates when it is ready.
- `WithDockerfileDirectory` builds a Docker image based on a Dockerfile (`ImageFromDockerfileBuilder`).
- `WithDeleteIfExists` removes the Docker image before it is rebuilt (`ImageFromDockerfileBuilder`).

## Pre-configured containers

The pre-configured Testcontainers below are supported. Further examples can be found in [TestcontainersContainerTest][1] as well as in [database][2] or [message broker][3] tests.

- CouchDB (couchdb:2.3.1)
- MsSql (server:2017-CU14-ubuntu)
- MySql (mysql:8.0.18)
- PostgreSql (postgres:11.5)
- Redis (redis:5.0.6)
- RabbitMQ (rabbitmq:3.7.21)

## Examples

Pulls `nginx`, creates a new container with port binding `80:80` and hits the default site.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage("nginx")
  .WithName("nginx")
  .WithPortBinding(80)
  .WithWaitStrategy(Wait.UntilPortsAreAvailable(80));

using (var testcontainer = testcontainersBuilder.Build())
{
  await testcontainer.StartAsync();
  var request = WebRequest.Create("http://localhost:80");
}
```

Mounts the current directory as volume into the container and runs `hostname > /tmp/hostname` on startup.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage("nginx")
  .WithName("nginx")
  .WithMount(".", "/tmp")
  .WithWaitStrategy(Wait.UntilFilesExists("/tmp/hostname"))
  .WithCommand("/bin/bash", "-c", "hostname > /tmp/hostname");

using (var testcontainer = testcontainersBuilder.Build())
{
  await testcontainer.StartAsync();
}
```

Here is an example of a pre-configured Testcontainer. In the example, Testcontainers starts a PostgreSQL database and executes a SQL query.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
  .WithDatabase(new PostgreSqlTestcontainerConfiguration
  {
    Database = "db",
    Username = "postgres",
    Password = "postgres",
  });

using (var testcontainer = testcontainersBuilder.Build())
{
  await testcontainer.StartAsync();

  using (var connection = new NpgsqlConnection(testcontainer.ConnectionString))
  {
    connection.Open();

    using (var cmd = new NpgsqlCommand())
    {
      cmd.Connection = connection;
      cmd.CommandText = "SELECT 1";
      cmd.ExecuteReader();
    }
  }
}
```

## Note

Please keep in mind this is not the official repository. Unfortunately, my requirements are not supported by the official implementation yet. Although we try to add new features and refactor the current version of [testcontainers/testcontainers-dotnet](https://github.com/testcontainers/testcontainers-dotnet), the progress is slow. As long as the official implementation does not cover all my requirements, I will work on both projects.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md)

## Authors

* **Andre Hofmeister** - *Initial work* - [HofmeisterAn](https://github.com/HofmeisterAn/)

## Thanks

Many thanks to [JetBrains](https://www.jetbrains.com/?from=dotnet-testcontainers) who provide an [Open Source License](https://www.jetbrains.com/community/opensource/) for this project :heart:.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

[1]: https://github.com/HofmeisterAn/dotnet-testcontainers/blob/develop/src/DotNet.Testcontainers.Tests/Unit/Linux/TestcontainersContainerTest.cs
[2]: https://github.com/HofmeisterAn/dotnet-testcontainers/blob/develop/src/DotNet.Testcontainers.Tests/Unit/Linux/Database
[3]: https://github.com/HofmeisterAn/dotnet-testcontainers/blob/develop/src/DotNet.Testcontainers.Tests/Unit/Linux/MessageBroker
