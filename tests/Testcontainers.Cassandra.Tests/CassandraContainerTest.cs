namespace Testcontainers.Cassandra.Tests
{
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
      var cluster = Cluster.Builder()
        .AddContactPoint(_cassandraContainer.GetEndPoint())
        .Build();

      // When
      var session = cluster.Connect();

      // Then
      Assert.True(session.GetState().GetConnectedHosts().First().IsUp);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DriverExecutesCqlStatementAndReturnResult()
    {
      // Given
      const string selectFromSystemLocalStatement = "SELECT * FROM system.local WHERE key = ?;";
      var cluster = Cluster.Builder()
        .AddContactPoint(_cassandraContainer.GetEndPoint())
        .Build();

      // When
      var session = await cluster.ConnectAsync();
      var preparedStatement = await session.PrepareAsync(selectFromSystemLocalStatement);
      var boundStatement = preparedStatement.Bind("local");
      var result = await session.ExecuteAsync(boundStatement);

      // Then
      Assert.True(result.IsFullyFetched);
      var resultRows = result.GetRows().ToList();
      Assert.Single(resultRows);
      Assert.Equal("COMPLETED", resultRows.First()["bootstrapped"]);
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
      Assert.NotEmpty(execResult.Stdout);
      Assert.Empty(execResult.Stderr);
    }
  }
}
