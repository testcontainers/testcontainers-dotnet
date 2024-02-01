namespace Testcontainers.CockroachDb;

public sealed class CockroachDbContainerTest : IAsyncLifetime
{
    private readonly CockroachDbContainer _cockroachDbContainer = new CockroachDbBuilder().Build();

    public Task InitializeAsync()
    {
        return _cockroachDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _cockroachDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new NpgsqlConnection(_cockroachDbContainer.GetConnectionString());

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
        var execResult = await _cockroachDbContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }
}