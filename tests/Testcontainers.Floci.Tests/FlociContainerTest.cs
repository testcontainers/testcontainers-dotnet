namespace Testcontainers.Floci;

public sealed class FlociContainerTest : IAsyncLifetime
{
    private const string AwsService = "Service";

    private static readonly BasicAWSCredentials AwsCredentials = new BasicAWSCredentials(FlociBuilder.AccessKey, FlociBuilder.SecretKey);

    private readonly FlociContainer _flociContainer = new FlociBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _flociContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _flociContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "cloudwatch")]
    public async Task CreateLogReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonCloudWatchLogsConfig();
        config.ServiceURL = _flociContainer.GetConnectionString();
        config.AuthenticationRegion = FlociBuilder.Region;

        using var client = new AmazonCloudWatchLogsClient(AwsCredentials, config);

        var logGroupRequest = new CreateLogGroupRequest(Guid.NewGuid().ToString("D"));

        // When
        var logGroupResponse = await client.CreateLogGroupAsync(logGroupRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, logGroupResponse.HttpStatusCode);
        Assert.Equal(_flociContainer.GetConnectionString(), _flociContainer.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "dynamodb")]
    public async Task GetItemReturnsPutItem()
    {
        // Given
        var id = Guid.NewGuid().ToString("D");

        var tableName = Guid.NewGuid().ToString("D");

        var config = new AmazonDynamoDBConfig();
        config.ServiceURL = _flociContainer.GetConnectionString();
        config.AuthenticationRegion = FlociBuilder.Region;

        using var client = new AmazonDynamoDBClient(AwsCredentials, config);

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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "s3")]
    public async Task ListBucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonS3Config();
        config.ServiceURL = _flociContainer.GetConnectionString();
        config.AuthenticationRegion = FlociBuilder.Region;

        using var client = new AmazonS3Client(AwsCredentials, config);

        // When
        var buckets = await client.ListBucketsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, buckets.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "sns")]
    public async Task CreateTopicReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonSimpleNotificationServiceConfig();
        config.ServiceURL = _flociContainer.GetConnectionString();
        config.AuthenticationRegion = FlociBuilder.Region;

        using var client = new AmazonSimpleNotificationServiceClient(AwsCredentials, config);

        // When
        var topicResponse = await client.CreateTopicAsync(Guid.NewGuid().ToString("D"), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, topicResponse.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "sqs")]
    public async Task CreateQueueReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonSQSConfig();
        config.ServiceURL = _flociContainer.GetConnectionString();
        config.AuthenticationRegion = FlociBuilder.Region;

        using var client = new AmazonSQSClient(AwsCredentials, config);

        // When
        var queueResponse = await client.CreateQueueAsync(Guid.NewGuid().ToString("D"), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, queueResponse.HttpStatusCode);
    }
}