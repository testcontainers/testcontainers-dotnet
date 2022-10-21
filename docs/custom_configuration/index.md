# Custom Configuration

Testcontainers for .NET supports various configurations to set up your test environment. It automatically discovers the Docker environment and applies the configuration. Set or override the default values either with the Testcontainers [properties file][c7974896] (`~/testcontainers.properties`) or with environment variables. If you prefer to configure your test environment at runtime, set or override the configuration through the `TestcontainersSettings` class. The following configurations are available:

| Properties File         | Environment Variable                   | Description                                                                                     |
|-------------------------|----------------------------------------|-------------------------------------------------------------------------------------------------|
| `docker.config`         | `DOCKER_CONFIG`                        | The directory path that contains the Docker configuration (`config.json`) file.                 |
| `docker.host`           | `DOCKER_HOST`                          | The Docker daemon socket to connect to.                                                         |
| `docker.auth.config`    | `DOCKER_AUTH_CONFIG`                   | The Docker configuration file content (GitLab: [use statically-defined credentials][e18c0570]). |
| `docker.cert.path`      | `DOCKER_CERT_PATH`                     | The directory path that contains the client certificate (`{ca,cert,key}.pem`) files.            |
| `docker.tls`            | `DOCKER_TLS`                           | Enables TLS.                                                                                    |
| `docker.tls.verify`     | `DOCKER_TLS_VERIFY`                    | Enables TLS verify.                                                                             |
| `ryuk.disabled`         | `TESTCONTAINERS_RYUK_DISABLED`         | Disables Ryuk (resource reaper).                                                                |
| `ryuk.container.image`  | `TESTCONTAINERS_RYUK_CONTAINER_IMAGE`  | The Ryuk (resource reaper) Docker image.                                                        |
| `hub.image.name.prefix` | `TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX` | The name to substitute the Docker Hub image name.                                               |

## Enable Logging

To enable logging, set the `TestcontainersSettings.Logger` property to an instance of an `ILogger` implementation. Either use your implementation or choose one of the common third-party providers.

### xUnit.net

The xUnit.net decorates are a convenient way to initialize your test environment just before the first test run. This includes logging. Configure your third-party provider as for any other .NET application, e.g. through the file or environment variables configuration provider. Set up and tear down your test environment with help of `Microsoft.Extensions.Hosting` that encapsulates resources and lifetime functionalities:

```csharp
/// <summary>
/// Runs once before the first test.
/// </summary>
[CollectionDefinition(nameof(SetUpTearDown))]
public sealed class SetUpTearDown : IClassFixture<SetUpTearDown>, IAsyncLifetime, IDisposable
{
  private readonly IHost host = Host
    .CreateDefaultBuilder()
    .ConfigureServices(serviceCollection =>
    {
      serviceCollection.AddSingleton<ILoggerFactory, CustomSerilogLoggerFactory>();
      serviceCollection.AddHostedService<CustomConfiguration>();
    })
    .Build();

  public Task InitializeAsync()
  {
    return this.host.StartAsync();
  }

  public Task DisposeAsync()
  {
    return this.host.StopAsync();
  }

  public void Dispose()
  {
    this.host.Dispose();
  }
}

/// <summary>
/// Sets the logger instance.
/// </summary>
public sealed class CustomConfiguration : IHostedService
{
  public CustomConfiguration(ILogger<CustomConfiguration> logger)
  {
    TestcontainersSettings.Logger = logger;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

/// <summary>
/// Assigns the test class to the SetUpTearDown collection.
/// </summary>
[Collection(nameof(SetUpTearDown))]
public sealed class Logger
{
  [Fact]
  public void Initialized()
  {
    Assert.IsType<Logger<CustomConfiguration>>(TestcontainersSettings.Logger);
  }
}
```

[c7974896]: https://docs.oracle.com/cd/E23095_01/Platform.93/ATGProgGuide/html/s0204propertiesfileformat01.html
[e18c0570]: https://docs.gitlab.com/ee/ci/docker/using_docker_images.html#use-statically-defined-credentials
