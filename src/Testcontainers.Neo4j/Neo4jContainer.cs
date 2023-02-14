namespace Testcontainers.Neo4j;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class Neo4jContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public Neo4jContainer(Neo4jConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}