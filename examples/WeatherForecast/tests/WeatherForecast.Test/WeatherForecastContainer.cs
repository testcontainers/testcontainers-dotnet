using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Xunit;

namespace WeatherForecast.Test;

[UsedImplicitly]
public sealed class WeatherForecastContainer : HttpClient, IAsyncLifetime
{
  private static readonly X509Certificate Certificate = new X509Certificate2(WeatherForecastImage.CertificateFilePath, WeatherForecastImage.CertificatePassword);

  private static readonly WeatherForecastImage Image = new();

  private readonly IDockerContainer _container;

  public WeatherForecastContainer()
    : base(new HttpClientHandler
    {
      ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
    })
  {
    _container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage(Image)
      .WithStartupCallback((_, ct) => Task.Delay(TimeSpan.FromSeconds(1), ct)) // For demonstration only, use a wait strategy instead.
      .WithPortBinding(WeatherForecastImage.HttpsPort, true)
      .WithEnvironment("ASPNETCORE_URLS", "https://+")
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", WeatherForecastImage.CertificateFilePath)
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", WeatherForecastImage.CertificatePassword)
      .Build();
  }

  public async Task InitializeAsync()
  {
    await Image.InitializeAsync()
      .ConfigureAwait(false);

    await _container.StartAsync()
      .ConfigureAwait(false);
  }

  public async Task DisposeAsync()
  {
    await Image.DisposeAsync()
      .ConfigureAwait(false);

    await _container.DisposeAsync()
      .ConfigureAwait(false);
  }

  public void SetBaseAddress()
  {
    try
    {
      var uriBuilder = new UriBuilder("https", _container.Hostname, _container.GetMappedPublicPort(WeatherForecastImage.HttpsPort));
      BaseAddress = uriBuilder.Uri;
    }
    catch
    {
      // Set the base address only once.
    }
  }
}
