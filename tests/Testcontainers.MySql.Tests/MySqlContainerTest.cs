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
        Assert.Equal(fixture.Container.GetConnectionString(), fixture.Container.GetConnectionString(ConnectionMode.Host));
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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptWithEncodingReturnsSuccessful()
    {
        // Given
        const string expectedValue = "Grüße";

        // The script declares Latin-1 as the client character set. Unless the script
        // content is written with the same encoding, MySql stores corrupted characters.
        const string scriptContent = "SET NAMES latin1; CREATE TABLE encoding_test (value VARCHAR(16)) CHARACTER SET utf8mb4; INSERT INTO encoding_test VALUES ('" + expectedValue + "');";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, Encoding.Latin1, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);

        using DbConnection connection = fixture.CreateConnection();
        await connection.OpenAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM encoding_test;";

        var actualValue = await command.ExecuteScalarAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        Assert.Equal(expectedValue, actualValue);
    }

    public class MySqlDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<MySqlBuilder, MySqlContainer>(messageSink)
    {
        protected override MySqlBuilder Configure()
            => new MySqlBuilder(TestSession.GetImageFromDockerfile());

        public override DbProviderFactory DbProviderFactory
            => MySqlConnectorFactory.Instance;
    }

    [UsedImplicitly]
    public class MySqlWaitForDatabaseFixture(IMessageSink messageSink)
        : MySqlDefaultFixture(messageSink)
    {
        protected override MySqlBuilder Configure()
            => base.Configure().WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public class MySqlRootFixture(IMessageSink messageSink)
        : MySqlDefaultFixture(messageSink)
    {
        protected override MySqlBuilder Configure()
            => base.Configure().WithUsername("root");
    }

    [UsedImplicitly]
    public class MySqlGitHubIssue1142Fixture(IMessageSink messageSink)
        : MySqlDefaultFixture(messageSink)
    {
        // https://github.com/testcontainers/testcontainers-dotnet/issues/1142.
        protected override MySqlBuilder Configure()
            => new MySqlBuilder(TestSession.GetImageFromDockerfile(stage: "v8_0_28"));
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