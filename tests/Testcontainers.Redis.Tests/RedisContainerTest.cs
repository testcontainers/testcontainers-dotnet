namespace Testcontainers.Redis;

public sealed class RedisContainerTest : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _redisContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _redisContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        using var connection = ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString());
        Assert.True(connection.IsConnected);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "return 'Hello, scripting!'";

        // When
        var execResult = await _redisContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.True("Hello, scripting!\n".Equals(execResult.Stdout), execResult.Stdout);
        Assert.Empty(execResult.Stderr);
    }
}