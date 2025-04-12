namespace Testcontainers.ClickHouse;

public abstract class ClickHouseContainerTest(ClickHouseContainerTest.ClickHouseDefaultFixture fixture)
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

    public class ClickHouseDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<ClickHouseBuilder, ClickHouseContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory
            => ClickHouseConnectionFactory.Instance;
    }

    [UsedImplicitly]
    public class ClickHouseWaitForDatabaseFixture(IMessageSink messageSink)
        : ClickHouseDefaultFixture(messageSink)
    {
        protected override ClickHouseBuilder Configure(ClickHouseBuilder builder)
            => builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class ClickHouseDefaultConfiguration(ClickHouseDefaultFixture fixture)
        : ClickHouseContainerTest(fixture), IClassFixture<ClickHouseDefaultFixture>;

    [UsedImplicitly]
    public sealed class ClickHouseWaitForDatabaseConfiguration(ClickHouseWaitForDatabaseFixture fixture)
        : ClickHouseContainerTest(fixture), IClassFixture<ClickHouseWaitForDatabaseFixture>;
}