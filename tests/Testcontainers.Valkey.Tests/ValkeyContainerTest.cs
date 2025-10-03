namespace Testcontainers.Valkey;

public sealed class ValkeyContainerTest : IAsyncLifetime
{
    private readonly ValkeyContainer _valkeyContainer = new ValkeyBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _valkeyContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _valkeyContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConnectionStateReturnsOpen()
    {
        using var connection = await ConnectionMultiplexer.ConnectAsync(_valkeyContainer.GetConnectionString());
        Assert.True(connection.IsConnected);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Can_Set_And_Retrieve_Key()
    {
      using var connection = await ConnectionMultiplexer.ConnectAsync(_valkeyContainer.GetConnectionString());

      var db = connection.GetDatabase();
      const string key = "test-key";
      const string value = "test-value";

      var redisValue = await db.StringGetAsync(key);
      Assert.True(redisValue.IsNull);

      await db.StringSetAsync(key, value);

      var updatedValue = await db.StringGetAsync(key);

      Assert.Equal(value, updatedValue);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "return 'Hello, Valkey!'";

        // When
        var execResult = await _valkeyContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.True("Hello, Valkey!\n".Equals(execResult.Stdout), execResult.Stdout);
        Assert.Empty(execResult.Stderr);
    }
}
