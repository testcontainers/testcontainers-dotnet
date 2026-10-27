namespace Testcontainers.MiniStack;

public sealed class MiniStackScriptTest : IAsyncLifetime
{
    // # --8<-- [start:ScriptMiniStackContainer]
    private readonly MiniStackContainer _miniStackContainer = new MiniStackBuilder(TestSession.GetImageFromDockerfile())
        .WithInitScriptContent("01-boot.sh", "echo booted > /tmp/booted")
        .WithReadyScriptContent("01-create-bucket.sh", "aws s3 mb s3://seeded-bucket")
        .WithReadyScript("02-create-queue.py")
        .Build();
    // # --8<-- [end:ScriptMiniStackContainer]

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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InitScriptRanBeforeStart()
    {
        // When
        var execResult = await _miniStackContainer.ExecAsync(new[] { "cat", "/tmp/booted" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal("booted", execResult.Stdout.Trim());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadyScriptsCreatedResourcesBeforeContainerIsReady()
    {
        // Given
        var s3Config = new AmazonS3Config();
        s3Config.ServiceURL = _miniStackContainer.GetConnectionString();
        s3Config.AuthenticationRegion = _miniStackContainer.GetRegion();
        s3Config.ForcePathStyle = true;

        var sqsConfig = new AmazonSQSConfig();
        sqsConfig.ServiceURL = _miniStackContainer.GetConnectionString();
        sqsConfig.AuthenticationRegion = _miniStackContainer.GetRegion();

        using var s3Client = new AmazonS3Client(Credentials, s3Config);

        using var sqsClient = new AmazonSQSClient(Credentials, sqsConfig);

        // When
        var listBucketsResponse = await s3Client.ListBucketsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync("seeded-queue", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains(listBucketsResponse.Buckets, bucket => "seeded-bucket".Equals(bucket.BucketName));
        Assert.EndsWith("/seeded-queue", getQueueUrlResponse.QueueUrl);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task StartThrowsWhenReadyScriptFails()
    {
        // Given
        await using var miniStackContainer = new MiniStackBuilder(TestSession.GetImageFromDockerfile())
            .WithReadyScriptContent("01-fail.sh", "exit 1")
            .Build();

        // When
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => miniStackContainer.StartAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.StartsWith("1 MiniStack ready script(s) failed.", exception.Message);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WithServicesLoadsOnlyTheGivenServices()
    {
        // Given
        await using var miniStackContainer = new MiniStackBuilder(TestSession.GetImageFromDockerfile())
            .WithServices("s3")
            .Build();

        await miniStackContainer.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var httpClient = new HttpClient();

        // When
        var health = await httpClient.GetStringAsync(new Uri(new Uri(miniStackContainer.GetConnectionString()), MiniStackBuilder.HealthPath), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var healthDocument = JsonDocument.Parse(health);

        // Then
        Assert.Equal(new[] { "s3" }, healthDocument.RootElement.GetProperty("services").EnumerateObject().Select(service => service.Name));
    }
}
