namespace Testcontainers.CockroachDb;

public abstract class CockroachDbContainerTest(CockroachDbContainerTest.CockroachDbDefaultFixture fixture)
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

    public class CockroachDbDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<CockroachDbBuilder, CockroachDbContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory
            => NpgsqlFactory.Instance;
    }

    [UsedImplicitly]
    public class CockroachDbWaitForDatabaseFixture(IMessageSink messageSink)
        : CockroachDbDefaultFixture(messageSink)
    {
        protected override CockroachDbBuilder Configure(CockroachDbBuilder builder)
            => builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class CockroachDbDefaultConfiguration(CockroachDbDefaultFixture fixture)
        : CockroachDbContainerTest(fixture), IClassFixture<CockroachDbDefaultFixture>;

    [UsedImplicitly]
    public sealed class CockroachDbWaitForDatabaseConfiguration(CockroachDbWaitForDatabaseFixture fixture)
        : CockroachDbContainerTest(fixture), IClassFixture<CockroachDbWaitForDatabaseFixture>;
}