namespace Testcontainers.FirebirdSql;

public abstract class FirebirdSqlContainerTest(FirebirdSqlContainerTest.FirebirdSqlDefaultFixture fixture)
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
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    public class FirebirdSqlDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<FirebirdSqlBuilder, FirebirdSqlContainer>(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile());

        public override DbProviderFactory DbProviderFactory
            => FirebirdClientFactory.Instance;
    }

    [UsedImplicitly]
    public class FirebirdSqlWaitForDatabaseFixture(IMessageSink messageSink)
        : FirebirdSqlDefaultFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile()).WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public class FirebirdSql25ScFixture(IMessageSink messageSink)
        : FirebirdSqlDefaultFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile(stage: "fb2.5-sc"));
    }

    [UsedImplicitly]
    public class FirebirdSql25SsFixture(IMessageSink messageSink)
        : FirebirdSqlDefaultFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile(stage: "fb2.5-ss"));
    }

    [UsedImplicitly]
    public class FirebirdSql30Fixture(IMessageSink messageSink)
        : FirebirdSqlDefaultFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile(stage: "fb3.0"));
    }

    [UsedImplicitly]
    public class FirebirdSqlSysdbaFixture(IMessageSink messageSink)
        : FirebirdSqlDefaultFixture(messageSink)
    {
        protected override FirebirdSqlBuilder Configure(FirebirdSqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile()).WithUsername("sysdba").WithPassword("some-password");
    }

    [UsedImplicitly]
    public sealed class FirebirdSqlDefaultConfiguration(FirebirdSqlDefaultFixture fixture)
        : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSqlDefaultFixture>;

    [UsedImplicitly]
    public sealed class FirebirdSqlWaitForDatabaseConfiguration(FirebirdSqlWaitForDatabaseFixture fixture)
        : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSqlWaitForDatabaseFixture>;

    [UsedImplicitly]
    public sealed class FirebirdSql25ScConfiguration(FirebirdSql25ScFixture fixture)
        : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSql25ScFixture>;

    [UsedImplicitly]
    public sealed class FirebirdSql25SsConfiguration(FirebirdSql25SsFixture fixture)
        : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSql25SsFixture>;

    [UsedImplicitly]
    public sealed class FirebirdSql30Configuration(FirebirdSql30Fixture fixture)
        : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSql30Fixture>;

    [UsedImplicitly]
    public sealed class FirebirdSqlSysdbaConfiguration(FirebirdSqlSysdbaFixture fixture)
        : FirebirdSqlContainerTest(fixture), IClassFixture<FirebirdSqlSysdbaFixture>;
}