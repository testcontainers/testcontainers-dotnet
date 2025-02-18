namespace Testcontainers.Cassandra;

public sealed class CassandraContainerTest : IAsyncLifetime
{
    private readonly CassandraContainer _cassandraContainer = new CassandraBuilder().Build();

    public Task InitializeAsync()
    {
        return _cassandraContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _cassandraContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new CqlConnection(_cassandraContainer.GetConnectionString());

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

        using var cluster = Cluster.Builder().WithConnectionString(_cassandraContainer.GetConnectionString()).Build();

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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptAsyncReturnsSuccess()
    {
        // Given
        const string selectFromSystemLocalStatement = "SELECT * FROM system.local;";

        // When
        var execResult = await _cassandraContainer.ExecScriptAsync(selectFromSystemLocalStatement)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
}