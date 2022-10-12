using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using JetBrains.Annotations;
using Xunit;

namespace WeatherForecast.Test;

[UsedImplicitly]
public sealed class WeatherForecastImage : IDockerImage, IAsyncLifetime
{
  public const ushort HttpsPort = 443;

  public const string CertificateFilePath = "certificate.crt";

  public const string CertificatePassword = "password";

  private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

  private readonly IDockerImage _image = new DockerImage(string.Empty, "weather-forecast", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

  public async Task InitializeAsync()
  {
    await _semaphoreSlim.WaitAsync()
      .ConfigureAwait(false);

    try
    {
      _ = await new ImageFromDockerfileBuilder()
        .WithName(this)
        .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
        .WithDockerfile("Dockerfile")
        .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
        .WithDeleteIfExists(false)
        .Build()
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
