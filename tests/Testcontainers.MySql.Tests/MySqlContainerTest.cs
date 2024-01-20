namespace Testcontainers.MySql;

public abstract class MySqlContainerTest : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;

    protected MySqlContainerTest(MySqlContainer mySqlContainer)
    {
        _mySqlContainer = mySqlContainer;
    }

    public Task InitializeAsync()
    {
        return _mySqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mySqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new MySqlConnection(_mySqlContainer.GetConnectionString());

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
        var execResult = await _mySqlContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class MySqlUserConfiguration : MySqlContainerTest
    {
        public MySqlUserConfiguration()
            : base(new MySqlBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class MySqlRootConfiguration : MySqlContainerTest
    {
        public MySqlRootConfiguration()
            : base(new MySqlBuilder().WithUsername("root").Build())
        {
        }
    }
}