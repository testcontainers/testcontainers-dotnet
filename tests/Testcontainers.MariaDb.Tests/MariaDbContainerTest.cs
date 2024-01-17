namespace Testcontainers.MariaDb;

public abstract class MariaDbContainerTest : IAsyncLifetime
{
    private readonly MariaDbContainer _mariaDbContainer;

    protected MariaDbContainerTest(MariaDbContainer mariaDbContainer)
    {
        _mariaDbContainer = mariaDbContainer;
    }

    public Task InitializeAsync()
    {
        return _mariaDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mariaDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new MySqlConnection(_mariaDbContainer.GetConnectionString());

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
        var execResult = await _mariaDbContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

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