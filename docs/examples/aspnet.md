# ASP.NET Core Blazor

No matter if your tests require databases, message brokers, your own services or even a running instance of your entire application, leveraging Testcontainers in your tests means you can set up the infrastructure fast and reliably. You can also run tests in parallel against multiple lightweight or a single shared heavyweight instance, depending on the use case. xUnit.net’s [shared context][xunit-shared-context] offers several methods to access resources efficiently among different tests and scopes.

The following example adds tests to an ASP.NET Core Blazor application. The tests cover the web front-end including the REST API of a weather forecast application. Testcontainers builds and ships our app in a Docker image, runs it in a Docker container, orchestrates the necessary resources, and executes the tests against it. This setup includes a Microsoft SQL Server to persist data and covers a common use case among many productive .NET applications. You find the entire example in the [testcontainers-dotnet][testcontainers-dotnet-weather-forecast-example] repository.

The `WeatherForecastContainer` class configures in the default constructor all dependencies to start the container that hosts our application.

```csharp
const string weatherForecastStorage = "weatherForecastStorage";

var mssqlConfiguration = new MsSqlTestcontainerConfiguration();
mssqlConfiguration.Password = Guid.NewGuid().ToString("D");
mssqlConfiguration.Database = Guid.NewGuid().ToString("D");

var connectionString = $"server={weatherForecastStorage};user id=sa;password={mssqlConfiguration.Password};database={mssqlConfiguration.Database}";

_weatherForecastNetwork = new NetworkBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .Build();

_mssqlContainer = new ContainerBuilder<MsSqlTestcontainer>()
  .WithDatabase(mssqlConfiguration)
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

await _mssqlContainer.StartAsync()
  .ConfigureAwait(false);

await _weatherForecastContainer.StartAsync()
  .ConfigureAwait(false);
```

xUnit.net passes the `WeatherForecastContainer` class fixture instance to the test class `WeatherForecastTest`. Each test collection `Api` and `Web` creates a new instance of `WeatherForecastContainer` and spins up an isolated environment. Even multiple test sessions do not interfere with each other. To run tests against a single shared heavyweight instance (collection fixture), add all dependencies to a collection definition. This works not only for containers, but also for any other Docker resource like images, networks or volumes.

As soon as the container is up and the application is running, each test sends an HTTP request to the weather forecast application and validates the REST or web front-end response. To visualize the web front-end, Selenium takes a screenshot right before and after the test.

[xunit-shared-context]: https://xunit.net/docs/shared-context
[testcontainers-dotnet-weather-forecast-example]: https://github.com/testcontainers/testcontainers-dotnet/tree/develop/examples/WeatherForecast
