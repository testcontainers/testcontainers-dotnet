namespace Testcontainers.FirebirdSql;

public abstract class FirebirdSqlContainerTest : IAsyncLifetime
{
    private readonly FirebirdSqlContainer _firebirdSqlContainer;

    private FirebirdSqlContainerTest(FirebirdSqlContainer firebirdSqlContainer)
    {
        _firebirdSqlContainer = firebirdSqlContainer;
    }

    public Task InitializeAsync()
    {
        return _firebirdSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _firebirdSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new FbConnection(_firebirdSqlContainer.GetConnectionString());

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
        const string scriptContent = "SELECT 1 FROM RDB$DATABASE;";

        // When
        var execResult = await _firebirdSqlContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class FirebirdSql25Sc : FirebirdSqlContainerTest
    {
        public FirebirdSql25Sc()
            : base(new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:2.5-sc").Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class FirebirdSql25Ss : FirebirdSqlContainerTest
    {
        public FirebirdSql25Ss()
            : base(new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:2.5-ss").Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class FirebirdSql30 : FirebirdSqlContainerTest
    {
        public FirebirdSql30()
            : base(new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:v3.0").Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class FirebirdSql40 : FirebirdSqlContainerTest
    {
        public FirebirdSql40()
            : base(new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:v4.0").Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class FirebirdSqlSysdba : FirebirdSqlContainerTest
    {
        public FirebirdSqlSysdba()
            : base(new FirebirdSqlBuilder().WithUsername("sysdba").WithPassword("some-password").Build())
        {
        }
    }
}