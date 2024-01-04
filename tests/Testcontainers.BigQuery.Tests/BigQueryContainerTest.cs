namespace Testcontainers.BigQuery;

public sealed class BigQueryContainerTest : IAsyncLifetime
{
    private readonly BigQueryContainer _bigQueryContainer = new BigQueryBuilder().Build();

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
    public async Task ExecuteQueryReturnsInsertRow()
    {
        // Given
        var utcNow = DateTime.UtcNow;

        // Storing DateTime.UtcNow in BigQuery loses precision. The query result differs
        // in the last digit. Truncating milliseconds prevents running into this issue.
        var utcNowWithoutMilliseconds = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);

        var bigQueryClientBuilder = new BigQueryClientBuilder();
        bigQueryClientBuilder.BaseUri = _bigQueryContainer.GetEmulatorEndpoint();
        bigQueryClientBuilder.ProjectId = BigQueryBuilder.DefaultProjectId;
        bigQueryClientBuilder.Credential = new Credential();

        var tableSchemaBuilder = new TableSchemaBuilder();
        tableSchemaBuilder.Add("player", BigQueryDbType.String);
        tableSchemaBuilder.Add("gameStarted", BigQueryDbType.DateTime);
        tableSchemaBuilder.Add("score", BigQueryDbType.Int64);
        var tableSchema = tableSchemaBuilder.Build();

        var expectedRow = new BigQueryInsertRow();
        expectedRow.Add("player", "Bob");
        expectedRow.Add("gameStarted", utcNowWithoutMilliseconds);
        expectedRow.Add("score", 85L);

        using var bigQueryClient = await bigQueryClientBuilder.BuildAsync()
            .ConfigureAwait(false);

        var dataset = await bigQueryClient.GetOrCreateDatasetAsync("mydata")
            .ConfigureAwait(false);

        // When
        var table = await dataset.CreateTableAsync("scores", tableSchema)
            .ConfigureAwait(false);

        _ = await table.InsertRowAsync(expectedRow)
            .ConfigureAwait(false);

        var results = await bigQueryClient.ExecuteQueryAsync($"SELECT * FROM {table}", null)
            .ConfigureAwait(false);

        // Then
        Assert.Single(results);
        Assert.Equal(expectedRow["player"], results.Single()["player"]);
        Assert.Equal(expectedRow["gameStarted"], results.Single()["gameStarted"]);
        Assert.Equal(expectedRow["score"], results.Single()["score"]);
    }

    private sealed class Credential : ICredential
    {
        public void Initialize(ConfigurableHttpClient httpClient)
        {
        }

        public Task<string> GetAccessTokenForRequestAsync(string authUri, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }
    }
}