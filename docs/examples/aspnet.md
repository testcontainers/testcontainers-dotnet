# ASP.NET Core

No matter if your tests require databases, message brokers, your own services or even a running instance of your entire application, leveraging Testcontainers in your tests means you can set up the infrastructure fast and reliably. You can also run tests in parallel against multiple lightweight or a single shared heavyweight instance, depending on the use case. xUnit.net's [shared context](https://xunit.net/docs/shared-context) and other test frameworks offers several methods to access resources efficiently among different tests and scopes.

## Utilizing WebApplicationFactory

ASP.NET Core offers the `WebApplicationFactory<TEntryPoint>` class for bootstrapping and creating a [TestServer](https://learn.microsoft.com/aspnet/core/test/integration-tests) dedicated to integration tests. There are several methods to integrate Testcontainers and dependent app services into ASP.NET Core integration tests.

One approach extends or overrides the application's configuration section to set up the [connection string](https://learn.microsoft.com/ef/core/miscellaneous/connection-strings) before creating the TestServer. This approach requires manually starting the dependent service in advance.

```csharp title="Set Redis connection string"
private sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseSetting("ConnectionStrings:RedisCache", _redisContainer.GetConnectionString());
  }
}
```

A slightly more complex approach involves implementing the `IConfigurationSource` interface and the `ConfigurationProvider` class. This approach adds a new configuration source and automatically starts the dependent service before creating the TestServer.

```csharp title="Add Redis configuration source"
private sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureAppConfiguration(configure =>
    {
      configure.Add(new RedisConfigurationSource());
    });
  }
}

private sealed class RedisConfigurationSource : IConfigurationSource
{
  public IConfigurationProvider Build(IConfigurationBuilder builder)
  {
    return new RedisConfigurationProvider();
  }
}

private sealed class RedisConfigurationProvider : ConfigurationProvider
{
  private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

  public override void Load()
  {
    // Until the asynchronous configuration provider is available,
    // we use the TaskFactory to spin up a new task that handles the work:
    // https://github.com/dotnet/runtime/issues/79193
    // https://github.com/dotnet/runtime/issues/36018
    TaskFactory.StartNew(LoadAsync)
      .Unwrap()
      .ConfigureAwait(false)
      .GetAwaiter()
      .GetResult();
  }

  public async Task LoadAsync()
  {
    var redisContainer = new RedisBuilder().Build();

    await redisContainer.StartAsync()
      .ConfigureAwait(false);

    Set("ConnectionStrings:RedisCache", redisContainer.GetConnectionString());
  }
}
```

## WeatherForecast Example

The following example adds tests to an ASP.NET Core Blazor application. The tests cover the web front-end including the REST API of a weather forecast application. Testcontainers builds and ships our app in a Docker image, runs it in a Docker container, orchestrates the necessary resources, and executes the tests against it. This setup includes a Microsoft SQL Server to persist data and covers a common use case among many productive .NET applications. You find the entire example in the [testcontainers-dotnet](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/examples/WeatherForecast) repository.

To use Testcontainers' pre-configured Microsoft SQL Server module, add the [Testcontainers.MsSql](https://www.nuget.org/packages/Testcontainers.MsSql) NuGet dependency to your test project:

```console
dotnet add package Testcontainers.MsSql --version 3.0.0
```

!!! note

    The Microsoft SQL Server Docker image is not compatible with ARM devices, such as Macs with Apple Silicon. Instead, you can use the [SqlEdge](https://www.nuget.org/packages/Testcontainers.SqlEdge) module or [Testcontainers Cloud](https://www.testcontainers.cloud/).

The `WeatherForecastContainer` class configures in the default constructor all dependencies to start the container that hosts our application.

```csharp
const string weatherForecastStorage = "weatherForecastStorage";

const string connectionString = $"server={weatherForecastStorage};user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database={MsSqlBuilder.DefaultDatabase}";

_weatherForecastNetwork = new NetworkBuilder()
  .Build();

_msSqlContainer = new MsSqlBuilder()
  .WithNetwork(_weatherForecastNetwork)
  .WithNetworkAliases(weatherForecastStorage)
  .Build();

_weatherForecastContainer = new ContainerBuilder()
  .WithImage(Image)
  .WithNetwork(_weatherForecastNetwork)
  .WithPortBinding(WeatherForecastImage.HttpsPort, true)
  .WithEnvironment("ASPNETCORE_URLS", "https://+")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", WeatherForecastImage.CertificateFilePath)
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", WeatherForecastImage.CertificatePassword)
  .WithEnvironment("ConnectionStrings__DefaultConnection", connectionString)
  .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(WeatherForecastImage.HttpsPort))
  .Build();
```

First, the class configures the Docker network. The application and database use this private network to communicate with each other. Followed by that, the class sets up the configuration of the Microsoft SQL Server container. Once all the dependencies are in place, the example configures the weather forecast application conveniently with environment variables. xUnit.net calls `IAsyncLifetime.InitializeAsync` immediately after the class has been created. The test project uses this mechanism to create and start all dependencies before any test run:

```csharp
await Image.InitializeAsync()
  .ConfigureAwait(false);

await _weatherForecastNetwork.CreateAsync()
  .ConfigureAwait(false);

await _msSqlContainer.StartAsync()
  .ConfigureAwait(false);

await _weatherForecastContainer.StartAsync()
  .ConfigureAwait(false);
```

xUnit.net passes the `WeatherForecastContainer` class fixture instance to the test class `WeatherForecastTest`. Each test collection `Api` and `Web` creates a new instance of `WeatherForecastContainer` and spins up an isolated environment. Even multiple test sessions do not interfere with each other. To run tests against a single shared heavyweight instance (collection fixture), add all dependencies to a collection definition. This works not only for containers, but also for any other Docker resource like images, networks or volumes.

As soon as the container is up and the application is running, each test sends an HTTP request to the weather forecast application and validates the REST or web front-end response. To visualize the web front-end, Selenium takes a screenshot right before and after the test.

Testcontainers not only works great for testing an application or service out-of-process, as shown in the example above, but it is probably even better for testing it in-process as shown in [this](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/examples/WeatherForecast/tests/WeatherForecast.InProcess.Tests) example. When used in conjunction with the ASP.NET `WebApplicationFactory<TEntryPoint>` class, Testcontainers spins up the dependent container together with the application, resulting in much faster tests. Depending on the application or service configuration, adding Testcontainers to bootstrap dependent services significantly improves the development experience. There is no longer a need to ensure that dependent services are running and wired up correctly on the development machine or CI environment.
