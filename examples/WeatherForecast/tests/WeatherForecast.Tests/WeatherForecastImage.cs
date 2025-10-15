namespace WeatherForecast.Tests;

[UsedImplicitly]
public sealed class WeatherForecastImage : IImage, IAsyncLifetime
{
  public const ushort HttpsPort = 443;

  public const string CertificateFilePath = "certificate.pfx";

  public const string CertificatePassword = "password";

  private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

  private readonly IImage _image = new DockerImage("localhost/testcontainers/weather-forecast", tag: DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

  public string Repository => _image.Repository;

  public string Registry => _image.Registry;

  public string Tag => _image.Tag;

  public string Digest => _image.Digest;

  public string FullName => _image.FullName;

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

  public string GetHostname()
  {
    return _image.GetHostname();
  }

  public bool MatchLatestOrNightly()
  {
    return _image.MatchLatestOrNightly();
  }

  public bool MatchVersion(Predicate<string> predicate)
  {
    return _image.MatchVersion(predicate);
  }

  public bool MatchVersion(Predicate<Version> predicate)
  {
    return _image.MatchVersion(predicate);
  }
}
