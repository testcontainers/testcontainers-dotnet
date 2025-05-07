namespace Testcontainers.DynamoDb;

public sealed class DynamoDbContainerTest : IAsyncLifetime
{
    private readonly DynamoDbContainer _dynamoDbContainer = new DynamoDbBuilder().Build();

    static DynamoDbContainerTest()
    {
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", CommonCredentials.AwsAccessKey);
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", CommonCredentials.AwsSecretKey);
    }

    public async ValueTask InitializeAsync()
    {
        await _dynamoDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _dynamoDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListBucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonDynamoDBConfig();
        config.ServiceURL = _dynamoDbContainer.GetConnectionString();

        using var client = new AmazonDynamoDBClient(config);

        // When
        var tables = await client.ListTablesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, tables.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetItemReturnsPutItem()
    {
        // Given
        var id = Guid.NewGuid().ToString("D");

        var tableName = Guid.NewGuid().ToString("D");

        var config = new AmazonDynamoDBConfig();
        config.ServiceURL = _dynamoDbContainer.GetConnectionString();

        using var client = new AmazonDynamoDBClient(config);

        var tableRequest = new CreateTableRequest();
        tableRequest.TableName = tableName;
        tableRequest.AttributeDefinitions = new List<AttributeDefinition> { new AttributeDefinition("Id", ScalarAttributeType.S) };
        tableRequest.KeySchema = new List<KeySchemaElement> { new KeySchemaElement("Id", KeyType.HASH) };
        tableRequest.ProvisionedThroughput = new ProvisionedThroughput(10, 5);

        var putItemRequest = new PutItemRequest();
        putItemRequest.TableName = tableName;
        putItemRequest.Item = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { S = id } } };

        var getItemRequest = new GetItemRequest();
        getItemRequest.TableName = tableName;
        getItemRequest.Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { S = id } } };

        // When
        _ = await client.CreateTableAsync(tableRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.PutItemAsync(putItemRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var itemResponse = await client.GetItemAsync(getItemRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(id, itemResponse.Item.Values.Single().S);
    }
}