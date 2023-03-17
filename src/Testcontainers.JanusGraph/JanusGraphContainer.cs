namespace Testcontainers.JanusGraph;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class JanusGraphContainer : DockerContainer
{

    private IGremlinClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public JanusGraphContainer(JanusGraphConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets a GraphTraversalSource to execute traversals.
    /// </summary>
    /// <returns>The GraphTraversalSource which uses a GremlinClient to connect to JanusGraph.</returns>
    public GraphTraversalSource GetGraphTraversalSource()
    {
        return Traversal().WithRemote(new DriverRemoteConnection(GetGremlinClient()));
    }

    /// <summary>
    /// Gets a connected GremlinClient.
    /// </summary>
    /// <returns>A GremlinClient which operates on this JanusGraph instance.</returns>
    private IGremlinClient GetGremlinClient()
    {
        return _client ?? new GremlinClient(new GremlinServer(Hostname, GetMappedPublicPort(JanusGraphBuilder.JanusGraphPort)), new JanusGraphGraphSONMessageSerializer());
    }
}
