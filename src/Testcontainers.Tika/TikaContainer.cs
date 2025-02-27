namespace Testcontainers.Tika;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class TikaContainer : DockerContainer
{
  private readonly TikaConfiguration _configuration;

  /// <summary>
  /// Initializes a new instance of the <see cref="TikaContainer" /> class.
  /// </summary>
  /// <param name="configuration">The container configuration.</param>
  public TikaContainer(TikaConfiguration configuration)
      : base(configuration)
  {
    _configuration = configuration;
  }

  /// <summary>
  /// Gets the Tika connection string.
  /// </summary>
  /// <returns>The Tika connection string.</returns>
  public string GetConnectionString()
  {
    var endpoint = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(TikaBuilder.TikaHttpPort));
    return endpoint.ToString();
  }
}
