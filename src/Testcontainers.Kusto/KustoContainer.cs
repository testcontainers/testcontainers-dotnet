namespace Testcontainers.Kusto;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class KustoContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KustoContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public KustoContainer(KustoConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the Kusto connection string.
    /// </summary>
    /// <returns>The Kusto connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(KustoBuilder.KustoPort)).ToString();
    }
}