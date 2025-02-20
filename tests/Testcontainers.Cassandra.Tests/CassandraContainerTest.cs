namespace Testcontainers.Cassandra;

public abstract class CassandraContainerTest(CassandraContainerTest.CassandraFixture fixture)
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
        var execResult = await fixture.Container.ExecScriptAsync(selectFromSystemLocalStatement)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class CassandraContainerDefaultConfiguration(CassandraFixture fixture) : CassandraContainerTest(fixture), IClassFixture<CassandraFixture>;

    [UsedImplicitly]
    public sealed class CassandraContainerWaitForDatabase(CassandraFixtureWaitForDatabase fixture) : CassandraContainerTest(fixture), IClassFixture<CassandraFixtureWaitForDatabase>;

    public class CassandraFixture(IMessageSink messageSink) : DbContainerFixture<CassandraBuilder, CassandraContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => CqlProviderFactory.Instance;
    }

    [UsedImplicitly]
    public sealed class CassandraFixtureWaitForDatabase(IMessageSink messageSink) : CassandraFixture(messageSink)
    {
        protected override CassandraBuilder Configure(CassandraBuilder builder)
        {
            return builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
        }
    }
}