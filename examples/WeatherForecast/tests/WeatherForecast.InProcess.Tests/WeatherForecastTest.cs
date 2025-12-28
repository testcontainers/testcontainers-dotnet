namespace WeatherForecast.InProcess.Tests;

[UsedImplicitly]
public sealed class WeatherForecastTest : IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

  public Task InitializeAsync()
  {
    return _postgreSqlContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _postgreSqlContainer.DisposeAsync().AsTask();
  }

  public sealed class Api : WebApplicationFactory<Program>, IClassFixture<WeatherForecastTest>
  {
    private readonly string _postgreSqlConnectionString;

    public Api(WeatherForecastTest weatherForecastTest)
    {
      _postgreSqlConnectionString = weatherForecastTest._postgreSqlContainer.GetConnectionString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.UseSetting("URLS", "https://+");
      builder.UseSetting("ConnectionStrings:PostgreSQL", _postgreSqlConnectionString);
    }

    [Fact]
    [Trait("Category", nameof(Api))]
    public async Task Get_WeatherForecast_ReturnsSevenDays()
    {
      // Given
      const string path = "api/WeatherForecast";

      using var httpClient = CreateClient();

      // When
      var response = await httpClient.GetAsync(path)
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

      using var serviceScope = Services.CreateScope();

      var weatherDataReadOnlyRepository = serviceScope.ServiceProvider.GetRequiredService<IWeatherDataReadOnlyRepository>();

      // When
      var weatherForecast = await weatherDataReadOnlyRepository.GetAllAsync(string.Empty, string.Empty, DateTime.Today, DateTime.Today.AddDays(threeDays))
        .ConfigureAwait(true);

      // Then
      Assert.Equal(threeDays, weatherForecast.Count());
    }
  }
}
