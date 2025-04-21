namespace Testcontainers.Tika.Tests;

public sealed class TikaContainerTests : IAsyncLifetime
{
  private readonly TikaContainer _tikaContainer = new TikaBuilder().Build();

  public Task InitializeAsync()
  {
    return _tikaContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _tikaContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task GetConnectionStringReturnsValidUrl()
  {
    // When
    var connectionString = await Task.Run(() => _tikaContainer.GetConnectionString());

    // Then
    Assert.False(string.IsNullOrEmpty(connectionString));
    Assert.StartsWith("http://", connectionString, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task TikaHealthCheckShouldBeSuccessful()
  {
    {
      // Given
      var httpClient = new HttpClient();
      var connectionString = await Task.Run(() => _tikaContainer.GetConnectionString());
      var requestUrl = $"{connectionString}tika";

      // When
      var response = await httpClient.GetAsync(requestUrl);

      // Then
      response.EnsureSuccessStatusCode();
      var content = await response.Content.ReadAsStringAsync();
      Assert.False(string.IsNullOrEmpty(content));
      Assert.StartsWith("This is Tika Server", content);
    }
  }
}
