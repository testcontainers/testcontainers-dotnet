namespace Testcontainers.Qdrant;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class QdrantContainer : DockerContainer
{
    private readonly QdrantConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public QdrantContainer(QdrantConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Qdrant connection url.
    /// </summary>
    /// <returns>The Qdrant connection url.</returns>
    public string GetConnectionUrl()
    {
        return $"http://{Hostname}:{GetMappedPublicPort(QdrantBuilder.QdrantPort)}";
    }
}
