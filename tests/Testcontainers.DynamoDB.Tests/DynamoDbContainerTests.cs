namespace Testcontainers.DynamoDB;

public sealed class MinioContainerTest : IAsyncLifetime
{
  private readonly DynamoDBContainer dynamoDbContainer = new DynamoDBBuilder().Build();

  public Task InitializeAsync()
  {
    return this.dynamoDbContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return this.dynamoDbContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task CreateTableReturnsCorrectTableDescription()
  {
    // Given
    const string tableName = "TestDynamoDbTable";
    using var client = this.dynamoDbContainer.GetAmazonDynamoDBClient();

    // When
    _ = await client.CreateTableAsync(new CreateTableRequest()
      {
        TableName = tableName,
        AttributeDefinitions = new List<AttributeDefinition>() { new AttributeDefinition("Id", ScalarAttributeType.S), new AttributeDefinition("Name", ScalarAttributeType.S), },
        KeySchema = new List<KeySchemaElement>() { new KeySchemaElement("Id", KeyType.HASH), new KeySchemaElement("Name", KeyType.RANGE), },
        ProvisionedThroughput = new ProvisionedThroughput(1, 1),
        TableClass = TableClass.STANDARD,
      })
      .ConfigureAwait(false);

    var tableDescription = await client.DescribeTableAsync(tableName).ConfigureAwait(false);

    // Then
    Assert.NotNull(tableDescription);
    Assert.Equal(HttpStatusCode.OK, tableDescription.HttpStatusCode);
    Assert.Equal(tableName, tableDescription.Table.TableName);
    Assert.Equal("Id", tableDescription.Table.KeySchema[0].AttributeName);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task InsertElementToTableReturnsHttpStatusCodeOk()
  {
    // Given
    var tableName = $"TestDynamoDbTable-{Guid.NewGuid():D}";
    var itemId = Guid.NewGuid().ToString("D");
    var itemName = Guid.NewGuid().ToString("D");

    using var client = this.dynamoDbContainer.GetAmazonDynamoDBClient();

    // When
    _ = await client.CreateTableAsync(new CreateTableRequest()
      {
        TableName = tableName,
        AttributeDefinitions = new List<AttributeDefinition>() { new AttributeDefinition("Id", ScalarAttributeType.S), new AttributeDefinition("Name", ScalarAttributeType.S), },
        KeySchema = new List<KeySchemaElement>() { new KeySchemaElement("Id", KeyType.HASH), new KeySchemaElement("Name", KeyType.RANGE), },
        ProvisionedThroughput = new ProvisionedThroughput(1, 1),
        TableClass = TableClass.STANDARD,
      })
      .ConfigureAwait(false);

    _ = await client.PutItemAsync(new PutItemRequest(tableName, new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue() { S = itemId } }, { "Name", new AttributeValue() { S = itemName } } })).ConfigureAwait(false);

    var getItemResponse = await client.GetItemAsync(new GetItemRequest(tableName, new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue() { S = itemId } }, { "Name", new AttributeValue() { S = itemName } } }))
      .ConfigureAwait(false);

    // Then
    Assert.Equal(HttpStatusCode.OK, getItemResponse.HttpStatusCode);
  }
}
