namespace Testcontainers.Dynalite;

using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

public sealed class MinioContainerTest : IAsyncLifetime
{
    private readonly DynaliteContainer _dynaliteContainer = new DynaliteBuilder().Build();

    public Task InitializeAsync()
    {
        return this._dynaliteContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return this._dynaliteContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateTableReturnsHttpStatusCodeOk()
    {
        // Given
        var clientConfig = new AmazonDynamoDBConfig();
        clientConfig.ServiceURL = this._dynaliteContainer.GetEndpoint();
        clientConfig.UseHttp = true;
        using var client = new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

        // When
        var tableResponse = await client.CreateTableAsync(new CreateTableRequest()
          {
            TableName = "TestDynamoDbTable",
            AttributeDefinitions = new List<AttributeDefinition>()
            {
              new AttributeDefinition("Id", ScalarAttributeType.N),
              new AttributeDefinition("Name", ScalarAttributeType.S),
            },
            KeySchema = new List<KeySchemaElement>()
            {
              new KeySchemaElement("Id", KeyType.HASH),
              new KeySchemaElement("Name", KeyType.RANGE),
            },
            ProvisionedThroughput = new ProvisionedThroughput(1, 1),
            TableClass = TableClass.STANDARD,
          })
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, tableResponse.HttpStatusCode);
    }

    // [Fact]
    // [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    // public async Task GetObjectReturnsPutObject()
    // {
    //     // Given
    //     using var inputStream = new MemoryStream(new byte[byte.MaxValue]);
    //
    //     var config = new AmazonS3Config();
    //     config.ServiceURL = this._dynaliteContainer.GetEndpoint();
    //
    //     var client = new AmazonS3Client(this._dynaliteContainer.GetAccessKeyId(), this._dynaliteContainer.GetAccessSecret(), config);
    //
    //     var objectRequest = new PutObjectRequest();
    //     objectRequest.BucketName = Guid.NewGuid().ToString("D");
    //     objectRequest.Key = Guid.NewGuid().ToString("D");
    //     objectRequest.InputStream = inputStream;
    //
    //     // When
    //     _ = await client.PutBucketAsync(objectRequest.BucketName)
    //         .ConfigureAwait(false);
    //
    //     _ = await client.PutObjectAsync(objectRequest)
    //         .ConfigureAwait(false);
    //
    //     var objectResponse = await client.GetObjectAsync(objectRequest.BucketName, objectRequest.Key)
    //         .ConfigureAwait(false);
    //
    //     // Then
    //     Assert.Equal(byte.MaxValue, objectResponse.ContentLength);
    // }
}
