namespace Testcontainers.BigQuery;

public sealed class BigQueryContainerTest : IAsyncLifetime
{
    const string ProjectId = "test-test";
    private readonly BigQueryContainer _bigQueryContainer = new BigQueryBuilder().WithProject(ProjectId).Build();

    public Task InitializeAsync()
    {
        return _bigQueryContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _bigQueryContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetSnapshotReturnsSetDocument()
    {
        string dataSetName = "mydata";
        string dataTableName = "scores";
        
        var firstDate = new DateTime(2000, 1, 14, 10, 30, 0, DateTimeKind.Utc);

        var bigQueryBuilder = new BigQueryClientBuilder();
        bigQueryBuilder.ProjectId = ProjectId;
        bigQueryBuilder.BaseUri = _bigQueryContainer.GetEmulatorEndpoint();

        var client = await bigQueryBuilder.BuildAsync()
            .ConfigureAwait(false);

        // Create the dataset if it doesn't exist.
        BigQueryDataset dataset = await client.GetOrCreateDatasetAsync(dataSetName);

        // Create the table if it doesn't exist.
        BigQueryTable table = await dataset.GetOrCreateTableAsync(dataTableName, new TableSchemaBuilder
        {
            {"player", BigQueryDbType.String},
            {"gameStarted", BigQueryDbType.Timestamp},
            {"score", BigQueryDbType.Int64}
        }.Build());

        // Insert a single row. There are many other ways of inserting
        // data into a table.
        await table.InsertRowAsync(new BigQueryInsertRow
        {
            {"player", "Bob"},
            {"score", 85},
            {"gameStarted", firstDate}
        });


        var data = await client.ExecuteQueryAsync($"select * from {ProjectId}.{dataSetName}.{dataTableName}",new BigQueryParameter[]{});
        
        Assert.Single(data);
        Assert.Equal(data.FirstOrDefault()!["player"],"Bob");
        Assert.Equal(data.FirstOrDefault()!["score"],(long)85);
        Assert.Equal(data.FirstOrDefault()!["gameStarted"],firstDate);
       
    }
}