namespace Testcontainers.JanusGraph;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class JanusGraphContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public JanusGraphContainer(JanusGraphConfiguration configuration)
        : base(configuration)
    {
    }
}