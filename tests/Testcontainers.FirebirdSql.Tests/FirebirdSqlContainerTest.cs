namespace Testcontainers.FirebirdSql;

public abstract class FirebirdSqlContainerTest(FirebirdSqlContainerTest.FirebirdSqlFixture fixture)
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = fixture.CreateConnection();

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
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    public abstract class FirebirdSqlFixture(IMessageSink messageSink) : DbContainerFixture<FirebirdSqlBuilder, FirebirdSqlContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => FirebirdClientFactory.Instance;
    }

    [UsedImplicitly]
    public sealed class FirebirdSqlDefault(FirebirdSqlDefaultFixture fixture) : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSqlDefaultFixture>;
    [UsedImplicitly]
    public sealed class FirebirdSqlDefaultFixture(IMessageSink messageSink) : FirebirdSqlFixture(messageSink);

    [UsedImplicitly]
    public sealed class FirebirdSql25Sc(FirebirdSql25ScFixture fixture) : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSql25ScFixture>;
    [UsedImplicitly]
    public sealed class FirebirdSql25ScFixture(IMessageSink messageSink) : FirebirdSqlFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder) => builder.WithImage("jacobalberty/firebird:2.5-sc");
    }

    [UsedImplicitly]
    public sealed class FirebirdSql25Ss(FirebirdSql25SsFixture fixture) : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSql25SsFixture>;
    [UsedImplicitly]
    public sealed class FirebirdSql25SsFixture(IMessageSink messageSink) : FirebirdSqlFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder) => builder.WithImage("jacobalberty/firebird:2.5-ss");
    }

    [UsedImplicitly]
    public sealed class FirebirdSql30(FirebirdSql30Fixture fixture) : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSql30Fixture>;
    [UsedImplicitly]
    public sealed class FirebirdSql30Fixture(IMessageSink messageSink) : FirebirdSqlFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder) => builder.WithImage("jacobalberty/firebird:v3.0");
    }

    [UsedImplicitly]
    public sealed class FirebirdSqlSysdba(FirebirdSqlSysdbaFixture fixture) : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSqlSysdbaFixture>;
    [UsedImplicitly]
    public sealed class FirebirdSqlSysdbaFixture(IMessageSink messageSink) : FirebirdSqlFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder) => builder.WithUsername("sysdba").WithPassword("some-password");
    }

    [UsedImplicitly]
    public sealed class FirebirdSqlWaitForDatabase(FirebirdSqlWaitForDatabaseFixture fixture) : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSqlWaitForDatabaseFixture>;
    [UsedImplicitly]
    public sealed class FirebirdSqlWaitForDatabaseFixture(IMessageSink messageSink) : FirebirdSqlFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder) => builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }
}