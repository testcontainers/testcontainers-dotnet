[![NuGet](https://img.shields.io/nuget/v/DotNet.Testcontainers.svg)](https://www.nuget.org/packages/DotNet.Testcontainers)
[![Build Status](https://dev.azure.com/HofmeisterAn/GitHub-Testcontainers/_apis/build/status/GitHub%20Testcontainers?branchName=develop)](https://dev.azure.com/HofmeisterAn/GitHub-Testcontainers/_build/latest?definitionId=6&branchName=develop)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=dotnet-testcontainers&metric=coverage)](https://sonarcloud.io/dashboard?id=dotnet-testcontainers)

# .NET Testcontainers
.NET Testcontainers is a library to support tests with throwaway instances of Docker containers. The library is built on top of the .NET Docker remote API and provides a lightweight implementation to support your test environment in all circumstances.

Choose from existing pre-configured configurations [^1] and start containers within a second, to support and run your tests.

## Examples
Pulls `nginx`, creates a new container with port binding `80:80` and hits the default site.

```csharp
var testcontainersBuilder = new TestcontainersBuilder()
  .WithImage("nginx")
  .WithName("nginx")
  .WithPortBinding(80);

using (var testcontainers = testcontainersBuilder.Build())
{
  testcontainers.Start();
  var request = WebRequest.Create($"http://localhost:80");
}
```

Mounts the current directory as volume into the container and runs `hostname > /tmp/hostname` on startup.

```csharp
var target = "tmp";

var file = "hostname";

var testcontainersBuilder = new TestcontainersBuilder()
  .WithImage("nginx")
  .WithName("nginx")
  .WithMount(".", $"/{target}")
  .WithCommand("/bin/bash", "-c", $"hostname > /{target}/{file}");

using (var testcontainers = testcontainersBuilder.Build())
{
  testcontainers.Start();
}
```

## Contributing

You are thinking about contributing to .NET Testcontainers? Awesome, it’s absolutely appreciated. To build the project just run the provided Cake script, `./build.sh` (Unix) or `.\build.ps1` (Windows).

* Fork the .NET Testcontainers repository.
* Create a branch to work with and use `feature/` or `bugfix/` as prefix.
* Don not forget the unit tests and keep the SonarCloud statistics in mind.
* If you are finished rebase and create a pull request.
* Cheers, :beers:.

## Authors

* **Andre Hofmeister** - *Initial work* - [HofmeisterAn](https://github.com/HofmeisterAn/)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

[^1]: I will add pre-configured configurations within the next releases.
