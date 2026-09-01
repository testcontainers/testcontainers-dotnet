namespace Testcontainers.MongoDb;

public sealed class MongoDbReplicaSetReuseTest : IAsyncLifetime
{
    private readonly string _labelKey = Guid.NewGuid().ToString("D");

    private readonly string _labelValue = Guid.NewGuid().ToString("D");

    private readonly IList<MongoDbContainer> _containers = new List<MongoDbContainer>();

    public async ValueTask InitializeAsync()
    {
        for (var _ = 0; _ < 3; _++)
        {
            var container = new MongoDbBuilder(TestSession.GetImageFromDockerfile())
                .WithReplicaSet()
                .WithLabel(_labelKey, _labelValue)
                .WithReuse(true)
                .Build();

            await container.StartAsync()
                .ConfigureAwait(false);

            _containers.Add(container);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_containers
            .Take(1)
            .Select(container =>
            {
                // We do not want to leak resources, but `WithCleanUp(true)` cannot be used
                // alongside `WithReuse(true)`. As a workaround, we set the `SessionId` using
                // reflection afterward to delete the container.
                container.AsDynamic()._configuration.SessionId = ResourceReaper.DefaultSessionId;
                return container.DisposeAsync().AsTask();
            }));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReusesSameContainerAcrossMultipleStarts()
    {
        // Given
        const string scriptContent = "rs.status().ok;";

        var mongoDbContainer = _containers[^1];

        // When
        var execResult = await mongoDbContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
        Assert.Single(_containers.Select(container => container.Id).Distinct());
    }
}