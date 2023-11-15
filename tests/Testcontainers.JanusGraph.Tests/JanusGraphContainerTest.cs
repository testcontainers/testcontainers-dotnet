namespace Testcontainers.JanusGraph;

public sealed class JanusGraphContainerTest : IAsyncLifetime
{
    private readonly JanusGraphContainer _janusGraphContainer = new JanusGraphBuilder().Build();

    public Task InitializeAsync()
    {
        return _janusGraphContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _janusGraphContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InsertedVertexCanBeFound()
    {
        // Given
        var label = Guid.NewGuid().ToString("D");

        using var client = new GremlinClient(new GremlinServer(_janusGraphContainer.Hostname, _janusGraphContainer.GetMappedPublicPort(JanusGraphBuilder.JanusGraphPort)), new JanusGraphGraphSONMessageSerializer());

        using var connection = new DriverRemoteConnection(client);

        var graphTraversalSource = AnonymousTraversalSource.Traversal().WithRemote(connection);

        // When
        await graphTraversalSource.AddV(label).Promise(traversal => traversal.Iterate())
            .ConfigureAwait(false);

        var count = await graphTraversalSource.V().HasLabel(label).Count().Promise(traversal => traversal.Next())
            .ConfigureAwait(false);

        // Then
        Assert.Equal(1, count);
    }
}