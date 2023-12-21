namespace Testcontainers.CockroachDb;

public sealed class CockroachDbContainerTest : IAsyncLifetime
{
    private readonly CockroachDbContainer _CockroachDbContainer = new CockroachDbBuilder().Build();

    public Task InitializeAsync()
    {
        return _CockroachDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _CockroachDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new NpgsqlConnection(_CockroachDbContainer.GetConnectionString());

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
        var execResult = await _CockroachDbContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(false);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }
}