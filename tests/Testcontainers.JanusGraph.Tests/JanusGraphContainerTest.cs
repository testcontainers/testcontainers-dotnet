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
        var g = _janusGraphContainer.GetGraphTraversalSource();
        await g.AddV("testLabel").Promise(t => t.Iterate());

        var actualCount = await g.V().HasLabel("testLabel").Count().Promise(t => t.Next());

        Assert.Equal(1, actualCount);
    }
}
