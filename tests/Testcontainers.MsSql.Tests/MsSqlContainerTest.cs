namespace Testcontainers.MsSql;

public abstract class MsSqlContainerTest(MsSqlContainerTest.MsSqlDefaultFixture fixture)
{
    // # --8<-- [start:UseMsSqlContainer]
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
    // # --8<-- [end:UseMsSqlContainer]

    public class MsSqlDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<MsSqlBuilder, MsSqlContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory
            => SqlClientFactory.Instance;
    }

    [UsedImplicitly]
    public class MsSqlWaitForDatabaseFixture(IMessageSink messageSink)
        : MsSqlDefaultFixture(messageSink)
    {
        protected override MsSqlBuilder Configure(MsSqlBuilder builder)
            => builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class MsSqlDefaultConfiguration(MsSqlDefaultFixture fixture)
        : MsSqlContainerTest(fixture), IClassFixture<MsSqlDefaultFixture>;

    [UsedImplicitly]
    public sealed class MsSqlWaitForDatabaseConfiguration(MsSqlWaitForDatabaseFixture fixture)
        : MsSqlContainerTest(fixture), IClassFixture<MsSqlWaitForDatabaseFixture>;
}