namespace Testcontainers.MySql;

public abstract class MySqlContainerTest(MySqlContainerTest.MySqlDefaultFixture fixture)
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
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    public class MySqlDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<MySqlBuilder, MySqlContainer>(messageSink)
    {
        protected override MySqlBuilder Configure(MySqlBuilder builder)
        {
            return builder.WithImage(TestSession.GetImageFromDockerfile());
        }

        public override DbProviderFactory DbProviderFactory
            => MySqlConnectorFactory.Instance;
    }

    [UsedImplicitly]
    public class MySqlWaitForDatabaseFixture(IMessageSink messageSink)
        : MySqlDefaultFixture(messageSink)
    {
        protected override MySqlBuilder Configure(MySqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile()).WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public class MySqlRootFixture(IMessageSink messageSink)
        : MySqlDefaultFixture(messageSink)
    {
        protected override MySqlBuilder Configure(MySqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile()).WithUsername("root");
    }

    [UsedImplicitly]
    public class MySqlGitHubIssue1142Fixture(IMessageSink messageSink)
        : MySqlDefaultFixture(messageSink)
    {
        // https://github.com/testcontainers/testcontainers-dotnet/issues/1142.
        protected override MySqlBuilder Configure(MySqlBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile(stage: "mysql8.0.28"));
    }

    [UsedImplicitly]
    public sealed class MySqlDefaultConfiguration(MySqlDefaultFixture fixture)
        : MySqlContainerTest(fixture), IClassFixture<MySqlDefaultFixture>;

    [UsedImplicitly]
    public sealed class MySqlWaitForDatabaseConfiguration(MySqlWaitForDatabaseFixture fixture)
        : MySqlContainerTest(fixture), IClassFixture<MySqlWaitForDatabaseFixture>;

    [UsedImplicitly]
    public sealed class MySqlRootConfiguration(MySqlRootFixture fixture)
        : MySqlContainerTest(fixture), IClassFixture<MySqlRootFixture>;

    [UsedImplicitly]
    public sealed class MySqlGitHubIssue1142Configuration(MySqlGitHubIssue1142Fixture fixture)
        : MySqlContainerTest(fixture), IClassFixture<MySqlGitHubIssue1142Fixture>;
}