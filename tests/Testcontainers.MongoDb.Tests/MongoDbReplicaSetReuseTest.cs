namespace Testcontainers.MongoDb;

/// <summary>
/// Reusing a container runs the startup callback again against a replica set that is already
/// initiated (https://github.com/testcontainers/testcontainers-dotnet/issues/1722).
/// </summary>
public sealed class MongoDbReplicaSetReuseTest : IAsyncLifetime
{
    private readonly string _labelKey = Guid.NewGuid().ToString("D");

    private readonly string _labelValue = Guid.NewGuid().ToString("D");

    private readonly IList<MongoDbContainer> _containers = new List<MongoDbContainer>();

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var container in _containers.Distinct())
        {
            await container.DisposeAsync()
                .ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReusedContainerStartsAgain()
    {
        // Given
        // The default wait strategy timeout is one hour, so bound the wait to keep a regression
        // from hanging the test run instead of failing it.
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        var container = CreateContainer();
        _containers.Add(container);

        await container.StartAsync(cts.Token)
            .ConfigureAwait(true);

        // When
        var reusedContainer = CreateContainer();
        _containers.Add(reusedContainer);

        await reusedContainer.StartAsync(cts.Token)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(container.Id, reusedContainer.Id);

        const string scriptContent = "rs.status().ok;";

        var execResult = await reusedContainer.ExecScriptAsync(scriptContent, cts.Token)
            .ConfigureAwait(true);

        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    private MongoDbContainer CreateContainer()
    {
        return new MongoDbBuilder(TestSession.GetImageFromDockerfile())
            .WithReplicaSet()
            .WithLabel(_labelKey, _labelValue)
            .WithReuse(true)
            .Build();
    }
}
