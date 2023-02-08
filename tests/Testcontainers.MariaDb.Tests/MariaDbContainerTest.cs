namespace Testcontainers.MariaDb;

public abstract class MariaDbContainerTest : IAsyncLifetime
{
    private readonly MariaDbContainer _mySqlContainer;

    protected MariaDbContainerTest(MariaDbContainer mySqlContainer)
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
            .ConfigureAwait(false);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class MariaDbUserConfiguration : MariaDbContainerTest
    {
        public MariaDbUserConfiguration()
            : base(new MariaDbBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class MariaDbRootConfiguration : MariaDbContainerTest
    {
        public MariaDbRootConfiguration()
            : base(new MariaDbBuilder().WithUsername("root").Build())
        {
        }
    }
}