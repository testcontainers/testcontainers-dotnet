using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Xunit;

namespace Testcontainers.Floci.Tests;

public sealed class FlociContainerTest : IAsyncLifetime
{
    private readonly FlociContainer _container = new FlociBuilder("floci/floci:1.5.13").Build();

    public ValueTask InitializeAsync() => new(_container.StartAsync());

    public ValueTask DisposeAsync() => _container.DisposeAsync();

    private AmazonS3Client CreateS3Client() =>
        new(
            new BasicAWSCredentials(_container.GetAccessKey(), _container.GetSecretKey()),
            new AmazonS3Config
            {
                ServiceURL = _container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
                ForcePathStyle = true
            });

    private AmazonSQSClient CreateSqsClient() =>
        new(
            new BasicAWSCredentials(_container.GetAccessKey(), _container.GetSecretKey()),
            new AmazonSQSConfig
            {
                ServiceURL = _container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region
            });

    private AmazonSimpleNotificationServiceClient CreateSnsClient() =>
        new(
            new BasicAWSCredentials(_container.GetAccessKey(), _container.GetSecretKey()),
            new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = _container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region
            });

    private AmazonDynamoDBClient CreateDynamoDbClient() =>
        new(
            new BasicAWSCredentials(_container.GetAccessKey(), _container.GetSecretKey()),
            new AmazonDynamoDBConfig
            {
                ServiceURL = _container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task S3_CreateBucketAndPutObject_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateS3Client();

        const string bucketName = "test-bucket";
        const string key = "test-key";
        const string content = "hello floci";

        await client.PutBucketAsync(new PutBucketRequest { BucketName = bucketName }, ct);
        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentBody = content,
        }, ct);

        var response = await client.GetObjectMetadataAsync(bucketName, key, ct);
        Assert.Equal(200, (int)response.HttpStatusCode);
    }

    [Fact]
    public async Task Sqs_CreateQueueAndSendMessage_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateSqsClient();

        var createResponse = await client.CreateQueueAsync(new CreateQueueRequest { QueueName = "test-queue" }, ct);
        var sendResponse = await client.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = createResponse.QueueUrl,
            MessageBody = "hello floci",
        }, ct);

        Assert.NotNull(sendResponse.MessageId);
    }

    [Fact]
    public async Task Sns_CreateTopicAndPublish_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateSnsClient();

        var createResponse = await client.CreateTopicAsync(new CreateTopicRequest { Name = "test-topic" }, ct);
        var publishResponse = await client.PublishAsync(new PublishRequest
        {
            TopicArn = createResponse.TopicArn,
            Message = "hello floci",
        }, ct);

        Assert.NotNull(publishResponse.MessageId);
    }

    [Fact]
    public async Task DynamoDb_CreateTableAndPutItem_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateDynamoDbClient();

        const string tableName = "test-table";

        await client.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            KeySchema = [new KeySchemaElement("Id", KeyType.HASH)],
            AttributeDefinitions = [new AttributeDefinition("Id", ScalarAttributeType.S)],
            BillingMode = BillingMode.PAY_PER_REQUEST,
        }, ct);

        var putResponse = await client.PutItemAsync(new PutItemRequest
        {
            TableName = tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["Id"] = new AttributeValue { S = "1" },
                ["Name"] = new AttributeValue { S = "Floci" },
            },
        }, ct);

        Assert.Equal(200, (int)putResponse.HttpStatusCode);
    }
}
