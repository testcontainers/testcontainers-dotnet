namespace Testcontainers.DragonflyDb;

public sealed class DragonflyDbContainerTest : IAsyncLifetime
{
    private readonly DragonflyDbContainer _dragonflyDbContainer = new DragonflyDbBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _dragonflyDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _dragonflyDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        using var connection = ConnectionMultiplexer.Connect(_dragonflyDbContainer.GetConnectionString());
        Assert.True(connection.IsConnected);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "return 'Hello, scripting!'";

        // When
        var execResult = await _dragonflyDbContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(0L, execResult.ExitCode);
        Assert.Equal("Hello, scripting!\n", execResult.Stdout);
        Assert.Empty(execResult.Stderr);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SetAndGetValueReturnsExpected()
    {
        // Given
        using var connection = ConnectionMultiplexer.Connect(_dragonflyDbContainer.GetConnectionString());
        var database = connection.GetDatabase();
        const string key = "test-key";
        const string value = "test-value";

        // When
        await database.StringSetAsync(key, value);
        var result = await database.StringGetAsync(key);

        // Then
        Assert.Equal(value, result);
    }
}