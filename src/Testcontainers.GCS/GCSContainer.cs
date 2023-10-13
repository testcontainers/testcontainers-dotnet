namespace Testcontainers.GCS;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class GCSContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GCSContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public GCSContainer(GCSConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the GCS connection string.
    /// </summary>
    /// <returns>The GCS connection string.</returns>
    public string GetConnectionString()
    {
        var builder = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(GCSBuilder.GCSPort), "storage/v1/");
        return builder.ToString();
    }
}