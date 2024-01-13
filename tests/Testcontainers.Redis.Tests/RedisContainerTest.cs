namespace Testcontainers.Redis;

public sealed class RedisContainerTest : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

    public Task InitializeAsync()
    {
        return _redisContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _redisContainer.DisposeAsync().AsTask();
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
        var execResult = await _redisContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True("Hello, scripting!\n".Equals(execResult.Stdout), execResult.Stdout);
    }
}