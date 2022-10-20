using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecast.Entities;
using WeatherForecast.Repositories;
using Xunit;

namespace WeatherForecast.InProcess.Test;

[UsedImplicitly]
public sealed class WeatherForecastTest : IAsyncLifetime
{
  private readonly MsSqlTestcontainer _mssqlContainer;

  public WeatherForecastTest()
  {
    var mssqlConfiguration = new MsSqlTestcontainerConfiguration();
    mssqlConfiguration.Password = Guid.NewGuid().ToString("D");
    mssqlConfiguration.Database = Guid.NewGuid().ToString("D");

    _mssqlContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
      .WithDatabase(mssqlConfiguration)
      .Build();
  }

  public Task InitializeAsync()
  {
    return _mssqlContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _mssqlContainer.DisposeAsync().AsTask();
  }

  public sealed class Api : IClassFixture<WeatherForecastTest>, IDisposable
  {
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private readonly IServiceScope _serviceScope;

    private readonly HttpClient _httpClient;

    public Api(WeatherForecastTest weatherForecastTest)
    {
      Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
      Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
      Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");
      Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", weatherForecastTest._mssqlContainer.ConnectionString);
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
        .ConfigureAwait(false);

      var weatherForecastStream = await response.Content.ReadAsStreamAsync()
        .ConfigureAwait(false);

      var weatherForecast = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherData>>(weatherForecastStream)
        .ConfigureAwait(false);

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
        .ConfigureAwait(false);

      // Then
      Assert.Equal(threeDays, weatherForecast.Count());
    }
  }
}
