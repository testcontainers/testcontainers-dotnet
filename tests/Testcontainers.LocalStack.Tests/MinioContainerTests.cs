using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Testcontainers.Minio;

public sealed class MinioContainerTest : IAsyncLifetime
{
    private readonly LocalStackContainer _localStackContainer = new LocalStackBuilder().WithServices(AwsService.S3, AwsService.DynamoDb).Build();

    public Task InitializeAsync()
    {
        return _localStackContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _localStackContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListBucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonS3Config();
        config.ServiceURL = _localStackContainer.GetEndpoint();

        var client = new AmazonS3Client(_localStackContainer.GetAccessKeyId(), _localStackContainer.GetAccessSecret(), config);

        // When
        var buckets = await client.ListBucketsAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, buckets.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetObjectReturnsPutObject()
    {
        // Given
        using var inputStream = new MemoryStream(new byte[byte.MaxValue]);

        var config = new AmazonS3Config();
        config.ServiceURL = _localStackContainer.GetEndpoint();

        var client = new AmazonS3Client(_localStackContainer.GetAccessKeyId(), _localStackContainer.GetAccessSecret(), config);

        var objectRequest = new PutObjectRequest();
        objectRequest.BucketName = Guid.NewGuid().ToString("D");
        objectRequest.Key = Guid.NewGuid().ToString("D");
        objectRequest.InputStream = inputStream;

        // When
        _ = await client.PutBucketAsync(objectRequest.BucketName)
            .ConfigureAwait(false);

        _ = await client.PutObjectAsync(objectRequest)
            .ConfigureAwait(false);

        var objectResponse = await client.GetObjectAsync(objectRequest.BucketName, objectRequest.Key)
            .ConfigureAwait(false);

        // Then
        Assert.Equal(byte.MaxValue, objectResponse.ContentLength);
    }
    
    
  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task CreateTableReturnsCorrectTableDescription()
  {
    // Given
    const string tableName = "TestDynamoDbTable";
    var clientConfig = new AmazonDynamoDBConfig();
    clientConfig.ServiceURL = this._localStackContainer.GetEndpoint();
    clientConfig.UseHttp = true;
    using var client =  new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

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

    var clientConfig = new AmazonDynamoDBConfig();
    clientConfig.ServiceURL = this._localStackContainer.GetEndpoint();
    clientConfig.UseHttp = true;
    using var client =  new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

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