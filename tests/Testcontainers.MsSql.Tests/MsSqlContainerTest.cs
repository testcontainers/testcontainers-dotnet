namespace Testcontainers.MsSql;

public abstract class MsSqlContainerTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;

    public MsSqlContainerTest(MsSqlContainer msSqlContainer)
    {
        _msSqlContainer = msSqlContainer;
    }

    // # --8<-- [start:UseMsSqlContainer]
    public Task InitializeAsync()
    {
        return _msSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new SqlConnection(_msSqlContainer.GetConnectionString());

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
        var execResult = await _msSqlContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UseMsSqlContainer]

    // # --8<-- [start:CreateMsSqlContainer]
    [UsedImplicitly]
    public sealed class MsSqlDefaultConfiguration : MsSqlContainerTest
    {
        public MsSqlDefaultConfiguration()
            : base(new MsSqlBuilder().Build())
        {
        }
    }
    // # --8<-- [end:CreateMsSqlContainer]
}