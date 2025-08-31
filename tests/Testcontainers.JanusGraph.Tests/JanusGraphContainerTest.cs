namespace Testcontainers.JanusGraph;

public sealed class JanusGraphContainerTest : IAsyncLifetime
{
    private readonly JanusGraphContainer _janusGraphContainer = new JanusGraphBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _janusGraphContainer.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _janusGraphContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InsertedVertexCanBeFound()
    {
        // Given
        var label = Guid.NewGuid().ToString("D");

        using var client = new GremlinClient(
            new GremlinServer(
                _janusGraphContainer.Hostname,
                _janusGraphContainer.GetMappedPublicPort(JanusGraphBuilder.JanusGraphPort)
            ),
            new JanusGraphGraphSONMessageSerializer()
        );

        using var connection = new DriverRemoteConnection(client);

        var graphTraversalSource = AnonymousTraversalSource.Traversal().WithRemote(connection);

        // When
        await graphTraversalSource
            .AddV(label)
            .Promise(traversal => traversal.Iterate(), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var count = await graphTraversalSource
            .V()
            .HasLabel(label)
            .Count()
            .Promise(traversal => traversal.Next(), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(1, count);
    }
}
