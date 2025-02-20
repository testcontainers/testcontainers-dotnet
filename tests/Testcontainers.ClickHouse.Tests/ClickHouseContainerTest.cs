namespace Testcontainers.ClickHouse;

public abstract class ClickHouseContainerTest(ClickHouseContainerTest.ClickHouseFixture fixture)
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
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class ClickHouseContainerDefaultConfiguration(ClickHouseFixture fixture) : ClickHouseContainerTest(fixture), IClassFixture<ClickHouseFixture>;

    [UsedImplicitly]
    public sealed class ClickHouseContainerWaitForDatabase(ClickHouseWaitForDatabaseFixture fixture) : ClickHouseContainerTest(fixture), IClassFixture<ClickHouseWaitForDatabaseFixture>;

    public class ClickHouseFixture(IMessageSink messageSink) : DbContainerFixture<ClickHouseBuilder, ClickHouseContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => ClickHouseConnectionFactory.Instance;
    }

    [UsedImplicitly]
    public sealed class ClickHouseWaitForDatabaseFixture(IMessageSink messageSink) : ClickHouseFixture(messageSink)
    {
        protected override ClickHouseBuilder Configure(ClickHouseBuilder builder)
        {
            return builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
        }
    }
}