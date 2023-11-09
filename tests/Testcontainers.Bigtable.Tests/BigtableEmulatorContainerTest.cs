namespace Testcontainers.Bigtable.Tests;

public class BigtableContainerTest : IAsyncLifetime
{
  private readonly BigtableEmulatorContainer _bigtableEmulatorContainer = new BigtableEmulatorBuilder().Build();

  public Task InitializeAsync()
  {
    return _bigtableEmulatorContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _bigtableEmulatorContainer.StopAsync();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public void AdminClientCanConnectAndCreateTable()
  {
    // Given
    var adminClient = new BigtableTableAdminClientBuilder { ChannelCredentials = ChannelCredentials.Insecure, Endpoint = _bigtableEmulatorContainer.GetEndpoint() }.Build();
    var instanceName = new InstanceName(_bigtableEmulatorContainer.ProjectId, _bigtableEmulatorContainer.InstanceId);
    var createTable = new Table
    {
      Granularity = Table.Types.TimestampGranularity.Unspecified,
      ColumnFamilies = { { "test", new ColumnFamily { GcRule = new GcRule { MaxNumVersions = 1 } } } }
    };
    var tableName = new TableName(_bigtableEmulatorContainer.ProjectId, _bigtableEmulatorContainer.InstanceId, "test-table");
    // When
    adminClient.CreateTable(instanceName, "test-table", createTable, CallSettings.FromCancellationToken(CancellationToken.None));
    var tableCreated = adminClient.GetTable(tableName);

    // Then
    Assert.NotNull(tableCreated);
  }
}
