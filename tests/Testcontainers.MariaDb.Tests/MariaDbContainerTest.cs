namespace Testcontainers.MariaDb;

public abstract class MariaDbContainerTest(MariaDbContainerTest.MariaDbDefaultFixture fixture)
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

    public class MariaDbDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<MariaDbBuilder, MariaDbContainer>(messageSink)
    {
        protected override MariaDbBuilder Configure(MariaDbBuilder builder)
        {
            return builder.WithImage(TestSession.GetImageFromDockerfile());
        }

        public override DbProviderFactory DbProviderFactory
            => MySqlConnectorFactory.Instance;
    }

    [UsedImplicitly]
    public class MariaDbWaitForDatabaseFixture(IMessageSink messageSink)
        : MariaDbDefaultFixture(messageSink)
    {
        protected override MariaDbBuilder Configure(MariaDbBuilder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile()).WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class MariaDbDefaultConfiguration(MariaDbDefaultFixture fixture)
        : MariaDbContainerTest(fixture), IClassFixture<MariaDbDefaultFixture>;

    [UsedImplicitly]
    public sealed class MariaDbWaitForDatabaseConfiguration(MariaDbWaitForDatabaseFixture fixture)
        : MariaDbContainerTest(fixture), IClassFixture<MariaDbWaitForDatabaseFixture>;
}