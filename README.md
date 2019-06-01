[![NuGet](https://img.shields.io/nuget/v/DotNet.Testcontainers.svg)](https://www.nuget.org/packages/DotNet.Testcontainers)
[![Build Status](https://dev.azure.com/HofmeisterAn/GitHub-Testcontainers/_apis/build/status/GitHub%20Testcontainers?branchName=develop)](https://dev.azure.com/HofmeisterAn/GitHub-Testcontainers/_build/latest?definitionId=6&branchName=develop)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=dotnet-testcontainers&metric=coverage)](https://sonarcloud.io/dashboard?id=dotnet-testcontainers)

# .NET Testcontainers

.NET Testcontainers is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions. The library is built on top of the .NET Docker remote API and provides a lightweight implementation to support your test environment in all circumstances.

Choose from existing pre-configured configurations and start containers within a second, to support and run your tests. Or create your own containers with Dockerfiles and run your tests immidiatly afterwards.

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
- `WithOutputConsumer` redirects `stdout` and `stderr` to capture the Testcontainer output.
- `WithWaitStrategy` sets the wait strategy to complete the Testcontainer start and indicates when it is ready.
- `WithDockerfileDirectory` builds a Docker image based on a Dockerfile (`ImageFromDockerfileBuilder`).
- `WithDeleteIfExists` removes the Docker image before it is rebuilt (`ImageFromDockerfileBuilder`).

## Pre-configured containers

The pre-configured Testcontainers below are supported. Further examples can be found in [TestcontainersContainerTest][1] and [DatabaseContainerTest][2].

- MsSql (server:2017-CU12-ubuntu)
- MySql (mysql:8.0.15)
- PostgreSql (postgres:11.2)

## Examples

Pulls `nginx`, creates a new container with port binding `80:80` and hits the default site.

```csharp
var testcontainersBuilder = new TestcontainersBuilder()
  .WithImage("nginx")
  .WithName("nginx")
  .WithPortBinding(80);

using (var testcontainer = testcontainersBuilder.Build())
{
  await testcontainer.Start();
  var request = WebRequest.Create("http://localhost:80");
}
```

Mounts the current directory as volume into the container and runs `hostname > /tmp/hostname` on startup.

```csharp
var testcontainersBuilder = new TestcontainersBuilder()
  .WithImage("nginx")
  .WithName("nginx")
  .WithMount(".", "/tmp")
  .WithCommand("/bin/bash", "-c", "hostname > /tmp/hostname");

using (var testcontainer = testcontainersBuilder.Build())
{
  await testcontainer.Start();
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

## Contributing

You are thinking about contributing to .NET Testcontainers? Awesome, it’s absolutely appreciated. To build the project just run the provided Cake script, `./build.sh` (Unix) or `.\build.ps1` (Windows).

* Fork the .NET Testcontainers repository.
* Create a branch to work with and use `feature/` or `bugfix/` as a prefix.
* Do not forget the unit tests and keep the SonarCloud statistics in mind.
* If you are finished rebase and create a pull request.
* Cheers, :beers:.

## Authors

* **Andre Hofmeister** - *Initial work* - [HofmeisterAn](https://github.com/HofmeisterAn/)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.


[1]: https://github.com/HofmeisterAn/dotnet-testcontainers/blob/develop/src/DotNet.Testcontainers.Tests/Unit/TestcontainersContainerTest.cs
[2]: https://github.com/HofmeisterAn/dotnet-testcontainers/blob/develop/src/DotNet.Testcontainers.Tests/Unit/DatabaseContainerTest.cs