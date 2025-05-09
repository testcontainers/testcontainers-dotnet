namespace Testcontainers.Cassandra;

public abstract class CassandraContainerTest(CassandraContainerTest.CassandraDefaultFixture fixture)
{
    // # --8<-- [start:UseCassandraContainer]
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
    public void ExecuteCqlStatementReturnsExpectedResult()
    {
        // Given
        const string selectFromSystemLocalStatement = "SELECT * FROM system.local WHERE key = ?;";

        using var cluster = Cluster.Builder().WithConnectionString(fixture.Container.GetConnectionString()).Build();

        // When
        using var session = cluster.Connect();

        var preparedStatement = session.Prepare(selectFromSystemLocalStatement);
        var boundStatement = preparedStatement.Bind("local");
        using var rowSet = session.Execute(boundStatement);
        var rows = rowSet.GetRows().ToImmutableList();

        // Then
        Assert.True(rowSet.IsFullyFetched);
        Assert.Single(rows);
        Assert.Equal("COMPLETED", rows[0]["bootstrapped"]);
    }
    // # --8<-- [end:UseCassandraContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptAsyncReturnsSuccess()
    {
        // Given
        const string selectFromSystemLocalStatement = "SELECT * FROM system.local;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(selectFromSystemLocalStatement, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    public class CassandraDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<CassandraBuilder, CassandraContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory
            => CqlProviderFactory.Instance;
    }

    [UsedImplicitly]
    public class CassandraWaitForDatabaseFixture(IMessageSink messageSink)
        : CassandraDefaultFixture(messageSink)
    {
        protected override CassandraBuilder Configure(CassandraBuilder builder)
            => builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class CassandraDefaultConfiguration(CassandraDefaultFixture fixture)
        : CassandraContainerTest(fixture), IClassFixture<CassandraDefaultFixture>;

    [UsedImplicitly]
    public sealed class CassandraWaitForDatabaseConfiguration(CassandraWaitForDatabaseFixture fixture)
        : CassandraContainerTest(fixture), IClassFixture<CassandraWaitForDatabaseFixture>;
}