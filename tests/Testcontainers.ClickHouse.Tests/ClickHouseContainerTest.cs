namespace Testcontainers.ClickHouse;

public sealed class ClickHouseContainerTest : IAsyncLifetime
{
    private readonly ClickHouseContainer _clickHouseContainer = new ClickHouseBuilder().Build();

    public Task InitializeAsync()
    {
        return _clickHouseContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _clickHouseContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new ClickHouseConnection(_clickHouseContainer.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await _clickHouseContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }
}