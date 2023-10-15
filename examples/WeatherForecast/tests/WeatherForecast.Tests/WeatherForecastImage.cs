namespace WeatherForecast.Tests;

[UsedImplicitly]
public sealed class WeatherForecastImage : IImage, IAsyncLifetime
{
  public const ushort HttpsPort = 443;

  public const string CertificateFilePath = "certificate.crt";

  public const string CertificatePassword = "password";

  private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

  private readonly IImage _image = new DockerImage("localhost/testcontainers", "weather-forecast", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

  public async Task InitializeAsync()
  {
    await _semaphoreSlim.WaitAsync()
      .ConfigureAwait(false);

    try
    {
      await new ImageFromDockerfileBuilder()
        .WithName(this)
        .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
        .WithDockerfile("Dockerfile")
        .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
        .WithDeleteIfExists(false)
        .Build()
        .CreateAsync()
        .ConfigureAwait(false);
    }
    finally
    {
      _semaphoreSlim.Release();
    }
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  public string Repository => _image.Repository;

  public string Name => _image.Name;

  public string Tag => _image.Tag;

  public string FullName => _image.FullName;

  public string GetHostname()
  {
    return _image.GetHostname();
  }
}
