namespace Testcontainers.Bigtable.Tests;

public class BigtableContainerTest : IAsyncLifetime
{
  private readonly BigtableContainer _bigtableContainer = new BigtableBuilder().Build();

  public Task InitializeAsync()
  {
    return _bigtableContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _bigtableContainer.StopAsync();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public void AdminClientCanConnectAndCreateTable()
  {
    // Given
    var adminClient = new BigtableTableAdminClientBuilder { ChannelCredentials = ChannelCredentials.Insecure, Endpoint = _bigtableContainer.GetEndpoint() }.Build();
    var instanceName = new InstanceName(_bigtableContainer.ProjectId, _bigtableContainer.InstanceId);
    var createTable = new Table
    {
      Granularity = Table.Types.TimestampGranularity.Unspecified,
      ColumnFamilies = { { "test", new ColumnFamily { GcRule = new GcRule { MaxNumVersions = 1 } } } }
    };
    var tableName = new TableName(_bigtableContainer.ProjectId, _bigtableContainer.InstanceId, "test-table");
    // When
    adminClient.CreateTable(instanceName, "test-table", createTable, CallSettings.FromCancellationToken(CancellationToken.None));
    var tableCreated = adminClient.GetTable(tableName);

    // Then
    Assert.NotNull(tableCreated);
  }
}
