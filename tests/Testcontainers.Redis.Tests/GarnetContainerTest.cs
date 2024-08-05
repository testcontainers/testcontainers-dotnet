namespace Testcontainers.Redis;

public sealed class GarnetContainerTest : IAsyncLifetime
{
    // see https://microsoft.github.io/garnet/docs/welcome/releases#docker
    private readonly RedisContainer _garnetContainer = new RedisBuilder()
        .WithImage("ghcr.io/microsoft/garnet")
        .Build();

    public Task InitializeAsync()
    {
        return _garnetContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _garnetContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        using var connection = ConnectionMultiplexer.Connect(_garnetContainer.GetConnectionString());
        Assert.True(connection.IsConnected);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "return 'Hello, scripting!'";

        // When
        var execResult = await _garnetContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.True("Hello, scripting!\n".Equals(execResult.Stdout), execResult.Stdout);
        Assert.Empty(execResult.Stderr);
    }
}
