namespace Testcontainers.MiniStack;

public abstract class MiniStackContainerTest : IAsyncLifetime
{
    private readonly MiniStackContainer _miniStackContainer;

    private MiniStackContainerTest(MiniStackContainer miniStackContainer)
    {
        _miniStackContainer = miniStackContainer;
    }

    // # --8<-- [start:UseMiniStackContainer]
    public async ValueTask InitializeAsync()
    {
        await _miniStackContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _miniStackContainer.DisposeAsync();
    }

    private AWSCredentials Credentials => new BasicAWSCredentials(_miniStackContainer.GetAccessKey(), _miniStackContainer.GetSecretKey());

    private TConfig GetConfig<TConfig>()
        where TConfig : ClientConfig, new()
    {
        var config = new TConfig();
        config.ServiceURL = _miniStackContainer.GetConnectionString();
        config.AuthenticationRegion = _miniStackContainer.GetRegion();
        return config;
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PutObjectReturnsSameObject()
    {
        // Given
        var config = GetConfig<AmazonS3Config>();
        config.ForcePathStyle = true;

        using var client = new AmazonS3Client(Credentials, config);

        var bucketName = Guid.NewGuid().ToString("D");

        // When
        // Outside us-east-1, S3 requires the bucket region as the location constraint.
        _ = await client.PutBucketAsync(new PutBucketRequest { BucketName = bucketName, BucketRegionName = _miniStackContainer.GetRegion() }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.PutObjectAsync(new PutObjectRequest { BucketName = bucketName, Key = "hello.txt", ContentBody = "world" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var getObjectResponse = await client.GetObjectAsync(bucketName, "hello.txt", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var streamReader = new StreamReader(getObjectResponse.ResponseStream);

        // Then
        Assert.Equal("world", await streamReader.ReadToEndAsync(TestContext.Current.CancellationToken));
        Assert.Equal(_miniStackContainer.GetConnectionString(), _miniStackContainer.GetConnectionString(ConnectionMode.Host));
    }
    // # --8<-- [end:UseMiniStackContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReceiveMessageReturnsSentMessage()
    {
        // Given
        using var client = new AmazonSQSClient(Credentials, GetConfig<AmazonSQSConfig>());

        var queueName = Guid.NewGuid().ToString("D");

        // When
        var createQueueResponse = await client.CreateQueueAsync(queueName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.SendMessageAsync(createQueueResponse.QueueUrl, "hello", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var receiveMessageResponse = await client.ReceiveMessageAsync(new ReceiveMessageRequest { QueueUrl = createQueueResponse.QueueUrl, MaxNumberOfMessages = 1 }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal("hello", Assert.Single(receiveMessageResponse.Messages).Body);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetItemReturnsPutItem()
    {
        // Given
        using var client = new AmazonDynamoDBClient(Credentials, GetConfig<AmazonDynamoDBConfig>());

        var id = Guid.NewGuid().ToString("D");

        var tableName = Guid.NewGuid().ToString("D");

        var tableRequest = new CreateTableRequest();
        tableRequest.TableName = tableName;
        tableRequest.BillingMode = BillingMode.PAY_PER_REQUEST;
        tableRequest.AttributeDefinitions = new List<AttributeDefinition> { new AttributeDefinition("Id", ScalarAttributeType.S) };
        tableRequest.KeySchema = new List<KeySchemaElement> { new KeySchemaElement("Id", KeyType.HASH) };

        var key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { S = id } } };

        // When
        _ = await client.CreateTableAsync(tableRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.PutItemAsync(tableName, key, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var getItemResponse = await client.GetItemAsync(tableName, key, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(id, getItemResponse.Item.Values.Single().S);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetMiniStackVersionReturnsImageTagAndServerVersion()
    {
        // Given
        var imageTag = new DockerImage(TestSession.GetImageFromDockerfile()).Tag;

        // When
        var serverVersion = await _miniStackContainer.GetMiniStackVersionAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(imageTag, _miniStackContainer.GetMiniStackVersion());
        Assert.True(System.Version.TryParse(serverVersion, out var version), $"'{serverVersion}' is not a version.");
        Assert.True(!System.Version.TryParse(imageTag, out var tagVersion) || tagVersion.Equals(version), $"The server reports {serverVersion} for the image tag {imageTag}.");
    }

    // # --8<-- [start:CreateMiniStackContainer]
    [UsedImplicitly]
    public sealed class MiniStackDefaultConfiguration : MiniStackContainerTest
    {
        public MiniStackDefaultConfiguration()
            : base(new MiniStackBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }
    // # --8<-- [end:CreateMiniStackContainer]

    [UsedImplicitly]
    public sealed class MiniStackCustomConfiguration : MiniStackContainerTest
    {
        private const string AccountId = "123456789012";

        private const string Region = "eu-west-1";

        // # --8<-- [start:CustomMiniStackContainer]
        public MiniStackCustomConfiguration()
            : base(new MiniStackBuilder(TestSession.GetImageFromDockerfile())
                .WithRegion(Region)
                .WithCredentials(AccountId, "secret")
                .WithPersistence()
                .WithImdsV2Required()
                .Build())
        {
        }
        // # --8<-- [end:CustomMiniStackContainer]

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task GetCallerIdentityReturnsConfiguredAccount()
        {
            // Given
            using var client = new AmazonSecurityTokenServiceClient(Credentials, GetConfig<AmazonSecurityTokenServiceConfig>());

            // When
            var getCallerIdentityResponse = await client.GetCallerIdentityAsync(new GetCallerIdentityRequest(), TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Equal(Region, _miniStackContainer.GetRegion());
            Assert.Equal(AccountId, _miniStackContainer.GetAccessKey());
            Assert.Equal(AccountId, getCallerIdentityResponse.Account);
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task QueueArnContainsConfiguredRegionAndAccount()
        {
            // Given
            using var client = new AmazonSQSClient(Credentials, GetConfig<AmazonSQSConfig>());

            // When
            var createQueueResponse = await client.CreateQueueAsync(Guid.NewGuid().ToString("D"), TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            var getQueueAttributesResponse = await client.GetQueueAttributesAsync(createQueueResponse.QueueUrl, new List<string> { "QueueArn" }, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.StartsWith($"arn:aws:sqs:{Region}:{AccountId}:", getQueueAttributesResponse.QueueARN);
        }

        [Theory]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        [InlineData("PERSIST_STATE", "1")]
        [InlineData("S3_PERSIST", "1")]
        [InlineData("MINISTACK_IMDS_V2_REQUIRED", "1")]
        [InlineData("MINISTACK_REGION", Region)]
        [InlineData("AWS_ACCESS_KEY_ID", AccountId)]
        [InlineData("AWS_SECRET_ACCESS_KEY", "secret")]
        public async Task EnvironmentVariableIsSet(string name, string expected)
        {
            // When
            var execResult = await _miniStackContainer.ExecAsync(new[] { "printenv", name }, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Equal(expected, execResult.Stdout.Trim());
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task MetadataRequestWithoutTokenReturnsHttpStatusCodeUnauthorized()
        {
            // Given
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_miniStackContainer.GetConnectionString());

            // When
            using var httpResponse = await httpClient.GetAsync("/latest/meta-data/", TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }
    }
}
