namespace Testcontainers.Bigtable;

public sealed class BigtableContainerTest : IAsyncLifetime
{
    private readonly BigtableContainer _bigtableContainer = new BigtableBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _bigtableContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _bigtableContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetTableReturnsCreateTable()
    {
        // Given
        var projectId = Guid.NewGuid().ToString("D");

        var instanceId = Guid.NewGuid().ToString("D");

        var tableId = Guid.NewGuid().ToString("D");

        var instanceName = new InstanceName(projectId, instanceId);

        var tableName = new TableName(projectId, instanceId, tableId);

        var columnFamily = new ColumnFamily();
        columnFamily.GcRule = new GcRule();
        columnFamily.GcRule.MaxNumVersions = 1;

        var table = new Table();
        table.Granularity = Table.Types.TimestampGranularity.Unspecified;
        table.ColumnFamilies.Add(nameof(columnFamily), columnFamily);

        var bigtableClientBuilder = new BigtableTableAdminClientBuilder();
        bigtableClientBuilder.Endpoint = _bigtableContainer.GetEmulatorEndpoint();
        bigtableClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        // When
        var bigtableClient = await bigtableClientBuilder.BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await bigtableClient.CreateTableAsync(instanceName, tableName.TableId, table)
            .ConfigureAwait(true);

        var actualTable = await bigtableClient.GetTableAsync(tableName)
            .ConfigureAwait(true);

        // Then
        Assert.NotNull(actualTable);
        Assert.Equal(projectId, actualTable.TableName.ProjectId);
        Assert.Equal(instanceId, actualTable.TableName.InstanceId);
        Assert.Equal(tableId, actualTable.TableName.TableId);
    }
}