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
using WeatherForecast.Entities;
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

  public sealed class Api : IClassFixture<WeatherForecastTest>
  {
    private readonly HttpClient _httpClient;

    public Api(WeatherForecastTest weatherForecastTest)
    {
      Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
      Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
      Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");
      Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", weatherForecastTest._mssqlContainer.ConnectionString);
      _httpClient = new WebApplicationFactory<Program>().CreateClient();
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
  }
}
