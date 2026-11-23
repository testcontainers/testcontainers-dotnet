namespace Testcontainers.AwsEmulator;

public sealed class AwsEmulatorContainerTest : IAsyncLifetime
{
    private const string AwsService = "Service";

    // # --8<-- [start:CreateAwsEmulatorContainer]
    private readonly AwsEmulatorContainer _awsEmulatorContainer = new AwsEmulatorBuilder(TestSession.GetImageFromDockerfile()).Build();
    // # --8<-- [end:CreateAwsEmulatorContainer]

    private AwsEmulatorHttpClientFactory _httpClientFactory;

    private AWSCredentials _awsCredentials;

    // # --8<-- [start:UseAwsEmulatorContainer]
    public async ValueTask InitializeAsync()
    {
        await _awsEmulatorContainer.StartAsync()
            .ConfigureAwait(false);

        // The emulator generates the certificate and the credentials during the
        // startup. Read them after the container has started.
        var certificate = await _awsEmulatorContainer.GetCertificateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var credentials = await _awsEmulatorContainer.GetCredentialsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _httpClientFactory = new AwsEmulatorHttpClientFactory(certificate);
        _awsCredentials = new BasicAWSCredentials(credentials.AccessKey, credentials.SecretKey);
    }

    public async ValueTask DisposeAsync()
    {
        _httpClientFactory?.Dispose();

        await _awsEmulatorContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "s3")]
    public async Task GetObjectReturnsPutObject()
    {
        // Given
        const string helloWorld = "Hello, World!";

        var bucketName = Guid.NewGuid().ToString("D");

        var objectKey = Guid.NewGuid().ToString("D");

        var config = new AmazonS3Config();
        config.ServiceURL = _awsEmulatorContainer.GetConnectionString();
        config.AuthenticationRegion = AwsEmulatorBuilder.Region;
        config.HttpClientFactory = _httpClientFactory;
        config.ForcePathStyle = true;

        using var client = new AmazonS3Client(_awsCredentials, config);

        var putObjectRequest = new PutObjectRequest();
        putObjectRequest.BucketName = bucketName;
        putObjectRequest.Key = objectKey;
        putObjectRequest.ContentBody = helloWorld;

        // When
        _ = await client.PutBucketAsync(bucketName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.PutObjectAsync(putObjectRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var objectResponse = await client.GetObjectAsync(bucketName, objectKey, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var streamReader = new StreamReader(objectResponse.ResponseStream);

        var content = await streamReader.ReadToEndAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloWorld, content);
        Assert.Equal(_awsEmulatorContainer.GetConnectionString(), _awsEmulatorContainer.GetConnectionString(ConnectionMode.Host));
    }
    // # --8<-- [end:UseAwsEmulatorContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "sqs")]
    public async Task ReceiveMessageReturnsSentMessage()
    {
        // Given
        const string helloWorld = "Hello, World!";

        var config = new AmazonSQSConfig();
        config.ServiceURL = _awsEmulatorContainer.GetConnectionString();
        config.AuthenticationRegion = AwsEmulatorBuilder.Region;
        config.HttpClientFactory = _httpClientFactory;

        using var client = new AmazonSQSClient(_awsCredentials, config);

        var receiveMessageRequest = new ReceiveMessageRequest();
        receiveMessageRequest.WaitTimeSeconds = 1;

        // When
        var queueResponse = await client.CreateQueueAsync(Guid.NewGuid().ToString("D"), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // The queue URL contains the container port. The client uses it to
        // identify the queue, but sends the requests to the service URL.
        receiveMessageRequest.QueueUrl = queueResponse.QueueUrl;

        _ = await client.SendMessageAsync(queueResponse.QueueUrl, helloWorld, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var messageResponse = await client.ReceiveMessageAsync(receiveMessageRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloWorld, Assert.Single(messageResponse.Messages).Body);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "s3")]
    public async Task ListBucketsThrowsAmazonServiceExceptionForInvalidCredentials()
    {
        // Given
        var awsCredentials = new BasicAWSCredentials("invalid", "invalid");

        var config = new AmazonS3Config();
        config.ServiceURL = _awsEmulatorContainer.GetConnectionString();
        config.AuthenticationRegion = AwsEmulatorBuilder.Region;
        config.HttpClientFactory = _httpClientFactory;
        config.ForcePathStyle = true;

        using var client = new AmazonS3Client(awsCredentials, config);

        // When
        var exception = await Assert.ThrowsAnyAsync<AmazonServiceException>(() => client.ListBucketsAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }
}