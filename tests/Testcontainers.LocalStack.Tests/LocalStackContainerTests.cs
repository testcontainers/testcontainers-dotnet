using System.Collections.Generic;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Testcontainers.Minio;

public sealed class LocalStackContainerTest : IAsyncLifetime
{
    private readonly LocalStackContainer _localStackContainer = new LocalStackBuilder().Build();

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
    public async Task ListAmazonS3BucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonS3Config();
        config.ServiceURL = _localStackContainer.GetEndpoint();

        var client = new AmazonS3Client(_localStackContainer.GetAccessKeyId(), _localStackContainer.GetAccessSecret(),
            config);

        // When
        var buckets = await client.ListBucketsAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, buckets.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateDynamoDbTableReturnsCorrectTableDescription()
    {
        // Given
        const string tableName = "TestDynamoDbTable";
        var clientConfig = new AmazonDynamoDBConfig();
        clientConfig.ServiceURL = this._localStackContainer.GetEndpoint();
        clientConfig.UseHttp = true;
        using var client = new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

        // When
        _ = await client.CreateTableAsync(new CreateTableRequest()
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition("Id", ScalarAttributeType.S),
                    new AttributeDefinition("Name", ScalarAttributeType.S),
                },
                KeySchema = new List<KeySchemaElement>()
                    { new KeySchemaElement("Id", KeyType.HASH), new KeySchemaElement("Name", KeyType.RANGE), },
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
    public async Task CreateSqsQueueReturnsHttpStatusCodeOk()
    {
        // Given
        string queueName = Guid.NewGuid().ToString("D");
        var clientConfig = new AmazonSQSConfig();
        clientConfig.ServiceURL = this._localStackContainer.GetEndpoint();
        clientConfig.UseHttp = true;
        using var client = new AmazonSQSClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

        // When
        var response = await client.CreateQueueAsync(new CreateQueueRequest(queueName));

        // Then
        Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateLogGroupReturnsHttpStatusCodeOk()
    {
        // Given
        string logGroupName = Guid.NewGuid().ToString("D");
        var clientConfig = new AmazonCloudWatchLogsConfig();
        clientConfig.ServiceURL = this._localStackContainer.GetEndpoint();
        clientConfig.UseHttp = true;
        using var client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

        // When
        var response = await client.CreateLogGroupAsync(new CreateLogGroupRequest(logGroupName));

        // Then
        Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
    }


    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateSnsTopicReturnsHttpStatusCodeOk()
    {
        // Given
        string topicName = Guid.NewGuid().ToString("D");
        var clientConfig = new AmazonSimpleNotificationServiceConfig();
        clientConfig.ServiceURL = this._localStackContainer.GetEndpoint();
        clientConfig.UseHttp = true;
        using var client =
            new AmazonSimpleNotificationServiceClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);

        // When
        var response = await client.CreateTopicAsync(new CreateTopicRequest(topicName));

        // Then
        Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
    }
}