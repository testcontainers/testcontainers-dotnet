namespace Testcontainers.Bigtable.Tests;

public class BigtableContainerTest : IAsyncLifetime
{
  private const string InstanceId = "instance-id";
  private const string ProjectId = "project-id";
  private const string TestTable = "test-table";
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
    var instanceName = new InstanceName(ProjectId, InstanceId);
    var createTable = new Table
    {
      Granularity = Table.Types.TimestampGranularity.Unspecified,
      ColumnFamilies = { { "test", new ColumnFamily { GcRule = new GcRule { MaxNumVersions = 1 } } } }
    };
    var tableName = new TableName(ProjectId, InstanceId, TestTable);
    // When
    adminClient.CreateTable(instanceName, TestTable, createTable, CallSettings.FromCancellationToken(CancellationToken.None));
    var tableCreated = adminClient.GetTable(tableName);

    // Then
    Assert.NotNull(tableCreated);
  }
}
