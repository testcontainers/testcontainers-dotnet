namespace WeatherForecast.Tests;

[UsedImplicitly]
public sealed class WeatherForecastContainer : HttpClient, IAsyncLifetime
{
  private static readonly X509Certificate Certificate = X509CertificateLoader.LoadPkcs12FromFile(WeatherForecastImage.CertificateFilePath, WeatherForecastImage.CertificatePassword);

  private static readonly WeatherForecastImage Image = new WeatherForecastImage();

  private readonly INetwork _weatherForecastNetwork;

  private readonly IContainer _postgreSqlContainer;

  private readonly IContainer _weatherForecastContainer;

  public WeatherForecastContainer()
    : base(new HttpClientHandler
    {
      // Trust the development certificate.
      ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
    })
  {
    const string weatherForecastStorage = "weatherForecastStorage";

    const string postgreSqlConnectionString = $"Host={weatherForecastStorage};Username={PostgreSqlBuilder.DefaultUsername};Password={PostgreSqlBuilder.DefaultPassword};Database={PostgreSqlBuilder.DefaultDatabase}";

    _weatherForecastNetwork = new NetworkBuilder()
      .Build();

    _postgreSqlContainer = new PostgreSqlBuilder("postgres:15.1")
      .WithNetwork(_weatherForecastNetwork)
      .WithNetworkAliases(weatherForecastStorage)
      .Build();

    _weatherForecastContainer = new ContainerBuilder(Image)
      .WithNetwork(_weatherForecastNetwork)
      .WithPortBinding(WeatherForecastImage.HttpsPort, true)
      .WithEnvironment("ASPNETCORE_URLS", "https://+")
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", WeatherForecastImage.CertificateFilePath)
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", WeatherForecastImage.CertificatePassword)
      .WithEnvironment("ConnectionStrings__PostgreSQL", postgreSqlConnectionString)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(WeatherForecastImage.HttpsPort))
      .Build();
  }

  public async Task InitializeAsync()
  {
    await Image.InitializeAsync()
      .ConfigureAwait(false);

    await _weatherForecastNetwork.CreateAsync()
      .ConfigureAwait(false);

    await _postgreSqlContainer.StartAsync()
      .ConfigureAwait(false);

    await _weatherForecastContainer.StartAsync()
      .ConfigureAwait(false);
  }

  public async Task DisposeAsync()
  {
    // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
    await Image.DisposeAsync()
      .ConfigureAwait(false);

    await _weatherForecastContainer.DisposeAsync()
      .ConfigureAwait(false);

    await _postgreSqlContainer.DisposeAsync()
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
