namespace Testcontainers.PostgreSql;

public sealed class PostgreSqlContainerTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());

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
        var execResult = await _postgreSqlContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task StopAndStartMultipleTimes()
    {
        // Given
        var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        // When
        var exception = await RestartAsync(timeoutSource.Token);

        // Then
        Assert.Null(exception);
    }

    private async Task<Exception> RestartAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 3; i++)
        {
            await _postgreSqlContainer.StopAsync(cancellationToken);
            var exception = await Record.ExceptionAsync(() => _postgreSqlContainer.StartAsync(cancellationToken));
            if (exception != null)
            {
                return exception;
            }
        }

        return null;
    }
}