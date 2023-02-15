namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ElasticsearchContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public ElasticsearchContainer(ElasticsearchConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}