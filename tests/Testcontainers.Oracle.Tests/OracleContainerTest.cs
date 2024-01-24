namespace Testcontainers.Oracle;

public sealed class OracleContainerTest : IAsyncLifetime
{
    private readonly OracleContainer _oracleContainer = new OracleBuilder().Build();

    public Task InitializeAsync()
    {
        return _oracleContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _oracleContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new OracleConnection(_oracleContainer.GetConnectionString());

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
        const string scriptContent = "SELECT 1 FROM DUAL;";

        // When
        var execResult = await _oracleContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }
}