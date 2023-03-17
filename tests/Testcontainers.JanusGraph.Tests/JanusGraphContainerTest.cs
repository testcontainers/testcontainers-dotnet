namespace Testcontainers.JanusGraph;

public sealed class JanusGraphContainerTest : IAsyncLifetime
{
    private readonly JanusGraphContainer _janusGraphContainer = new JanusGraphBuilder().Build();

    private IGremlinClient _client;

    public Task InitializeAsync()
    {
        return _janusGraphContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _janusGraphContainer.DisposeAsync().AsTask();
    }

    private GraphTraversalSource GraphTraversalSource => Traversal().WithRemote(new DriverRemoteConnection(GremlinClient()));

    private IGremlinClient GremlinClient()
    {
        _client ??= new GremlinClient(new GremlinServer(_janusGraphContainer.Hostname,
          _janusGraphContainer.GetMappedPublicPort(JanusGraphBuilder.JanusGraphPort)), new JanusGraphGraphSONMessageSerializer());
        return _client;
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InsertedVertexCanBeFound()
    {
        var g = GraphTraversalSource;
        await g.AddV("testLabel").Promise(t => t.Iterate());

        var actualCount = await g.V().HasLabel("testLabel").Count().Promise(t => t.Next());

        Assert.Equal(1, actualCount);
    }
}
