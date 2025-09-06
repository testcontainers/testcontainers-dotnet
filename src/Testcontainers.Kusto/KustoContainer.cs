namespace Testcontainers.Kusto;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class KustoContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KustoContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public KustoContainer(KustoConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the Kusto connection string.
    /// </summary>
    /// <returns>The Kusto connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(KustoBuilder.KustoPort)
        ).ToString();
    }
}
