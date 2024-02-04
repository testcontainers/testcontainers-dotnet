namespace WeatherForecast.InProcess.Tests;

[UsedImplicitly]
public sealed class WeatherForecastTest : IAsyncLifetime
{
  private readonly SqlEdgeContainer _sqlEdgeContainer = new SqlEdgeBuilder().Build();

  public Task InitializeAsync()
  {
    return _sqlEdgeContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _sqlEdgeContainer.DisposeAsync().AsTask();
  }

  public sealed class Api : IClassFixture<WeatherForecastTest>, IDisposable
  {
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private readonly IServiceScope _serviceScope;

    private readonly HttpClient _httpClient;

    public Api(WeatherForecastTest weatherForecastTest)
    {
      // Instead of using environment variables to bootstrap our application configuration, we can implement a custom WebApplicationFactory<TEntryPoint>
      // that overrides the ConfigureWebHost(IWebHostBuilder) method to add a WeatherDataContext to the service collection.
      Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
      Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
      Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");
      Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", weatherForecastTest._sqlEdgeContainer.GetConnectionString());
      _webApplicationFactory = new WebApplicationFactory<Program>();
      _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
      _httpClient = _webApplicationFactory.CreateClient();
    }

    public void Dispose()
    {
      _httpClient.Dispose();
      _serviceScope.Dispose();
      _webApplicationFactory.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(Api))]
    public async Task Get_WeatherForecast_ReturnsSevenDays()
    {
      // Given
      const string path = "api/WeatherForecast";

      // When
      var response = await _httpClient.GetAsync(path)
        .ConfigureAwait(true);

      var weatherForecastStream = await response.Content.ReadAsStreamAsync()
        .ConfigureAwait(true);

      var weatherForecast = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherData>>(weatherForecastStream)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      Assert.Equal(7, weatherForecast!.Count());
    }

    [Fact]
    [Trait("Category", nameof(Api))]
    public async Task Get_WeatherForecast_ReturnsThreeDays()
    {
      // Given
      const int threeDays = 3;

      var weatherDataReadOnlyRepository = _serviceScope.ServiceProvider.GetRequiredService<IWeatherDataReadOnlyRepository>();

      // When
      var weatherForecast = await weatherDataReadOnlyRepository.GetAllAsync(string.Empty, string.Empty, DateTime.Today, DateTime.Today.AddDays(threeDays))
        .ConfigureAwait(true);

      // Then
      Assert.Equal(threeDays, weatherForecast.Count());
    }
  }
}
