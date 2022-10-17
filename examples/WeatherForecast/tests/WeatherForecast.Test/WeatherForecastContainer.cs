using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using JetBrains.Annotations;
using Xunit;

namespace WeatherForecast.Test;

[UsedImplicitly]
public sealed class WeatherForecastContainer : HttpClient, IAsyncLifetime
{
  private static readonly X509Certificate Certificate = new X509Certificate2(WeatherForecastImage.CertificateFilePath, WeatherForecastImage.CertificatePassword);

  private static readonly WeatherForecastImage Image = new();

  private readonly IDockerNetwork _weatherForecastNetwork;

  private readonly IDockerContainer _mssqlContainer;

  private readonly IDockerContainer _weatherForecastContainer;

  public WeatherForecastContainer()
    : base(new HttpClientHandler
    {
      // Trust the development certificate.
      ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
    })
  {
    const string weatherForecastStorage = "weatherForecastStorage";

    var mssqlConfiguration = new MsSqlTestcontainerConfiguration();
    mssqlConfiguration.Password = Guid.NewGuid().ToString("D");
    mssqlConfiguration.Database = Guid.NewGuid().ToString("D");

    var connectionString = $"server={weatherForecastStorage};user id=sa;password={mssqlConfiguration.Password};database={mssqlConfiguration.Database}";

    _weatherForecastNetwork = new TestcontainersNetworkBuilder()
      .WithName(Guid.NewGuid().ToString("D"))
      .Build();

    _mssqlContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
      .WithDatabase(mssqlConfiguration)
      .WithNetwork(_weatherForecastNetwork)
      .WithNetworkAliases(weatherForecastStorage)
      .Build();

    _weatherForecastContainer = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage(Image)
      .WithNetwork(_weatherForecastNetwork)
      .WithPortBinding(WeatherForecastImage.HttpsPort, true)
      .WithEnvironment("ASPNETCORE_URLS", "https://+")
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", WeatherForecastImage.CertificateFilePath)
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", WeatherForecastImage.CertificatePassword)
      .WithEnvironment("ConnectionStrings__DefaultConnection", connectionString)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(WeatherForecastImage.HttpsPort))
      .Build();
  }

  public async Task InitializeAsync()
  {
    // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
    await Image.InitializeAsync()
      .ConfigureAwait(false);

    await _weatherForecastNetwork.CreateAsync()
      .ConfigureAwait(false);

    await _mssqlContainer.StartAsync()
      .ConfigureAwait(false);

    await _weatherForecastContainer.StartAsync()
      .ConfigureAwait(false);
  }

  public async Task DisposeAsync()
  {
    await Image.DisposeAsync()
      .ConfigureAwait(false);

    await _weatherForecastContainer.DisposeAsync()
      .ConfigureAwait(false);

    await _mssqlContainer.DisposeAsync()
      .ConfigureAwait(false);

    await _weatherForecastNetwork.DeleteAsync()
      .ConfigureAwait(false);
  }

  public void SetBaseAddress()
  {
    try
    {
      var uriBuilder = new UriBuilder("https", _weatherForecastContainer.Hostname, _weatherForecastContainer.GetMappedPublicPort(WeatherForecastImage.HttpsPort));
      BaseAddress = uriBuilder.Uri;
    }
    catch
    {
      // Set the base address only once.
    }
  }
}
