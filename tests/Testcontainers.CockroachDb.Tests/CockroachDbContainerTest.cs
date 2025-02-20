namespace Testcontainers.CockroachDb;

public abstract class CockroachDbContainerTest(CockroachDbContainerTest.CockroachDbFixture fixture)
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
    public sealed class CockroachDbContainerDefaultConfiguration(CockroachDbFixture fixture) : CockroachDbContainerTest(fixture), IClassFixture<CockroachDbFixture>;

    [UsedImplicitly]
    public sealed class CockroachDbContainerWaitForDatabase(CockroachDbFixtureWaitForDatabase fixture) : CockroachDbContainerTest(fixture), IClassFixture<CockroachDbFixtureWaitForDatabase>;

    public class CockroachDbFixture(IMessageSink messageSink) : DbContainerFixture<CockroachDbBuilder, CockroachDbContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;
    }

    [UsedImplicitly]
    public sealed class CockroachDbFixtureWaitForDatabase(IMessageSink messageSink) : CockroachDbFixture(messageSink)
    {
        protected override CockroachDbBuilder Configure(CockroachDbBuilder builder)
        {
            return builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
        }
    }
}