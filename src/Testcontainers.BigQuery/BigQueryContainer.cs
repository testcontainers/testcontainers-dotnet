namespace Testcontainers.BigQuery;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class BigQueryContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public BigQueryContainer(BigQueryConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the BigQuery emulator endpoint.
    /// </summary>
    /// <returns>The BigQuery emulator endpoint.</returns>
    public string GetEmulatorEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(BigQueryBuilder.BigQueryPort)).ToString();
    }
}