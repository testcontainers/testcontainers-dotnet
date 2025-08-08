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
    public void ConnectionStateReturnsOpen()
    {
        using var connection = ConnectionMultiplexer.Connect(_valkeyContainer.GetConnectionString());
        Assert.True(connection.IsConnected);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void Can_Set_And_Retrieve_Key()
    {
      using var connection = ConnectionMultiplexer.Connect(_valkeyContainer.GetConnectionString());

      var db = connection.GetDatabase();
      const string key = "test-key";
      const string value = "test-value";

      Assert.True(db.StringGet(key).IsNull);

      db.StringSet(key, value);

      var retrievedValue = db.StringGet(key);

      Assert.Equal(value, retrievedValue);
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
