namespace Testcontainers.RustFs;

public sealed class RustFsContainerTest : IAsyncLifetime
{
    private readonly RustFsContainer _rustFsContainer = new RustFsBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _rustFsContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _rustFsContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListBucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonS3Config();
        config.ServiceURL = _rustFsContainer.GetConnectionString();
        config.AuthenticationRegion = "us-east-1";
        config.ForcePathStyle = true;

        using var client = new AmazonS3Client(_rustFsContainer.GetAccessKey(), _rustFsContainer.GetSecretKey(), config);

        // When
        var buckets = await client.ListBucketsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, buckets.HttpStatusCode);
        Assert.Equal(_rustFsContainer.GetConnectionString(), _rustFsContainer.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetObjectReturnsPutObject()
    {
        // Given
        using var inputStream = new MemoryStream(new byte[byte.MaxValue]);

        var config = new AmazonS3Config();
        config.ServiceURL = _rustFsContainer.GetConnectionString();
        config.AuthenticationRegion = "us-east-1";
        config.ForcePathStyle = true;

        using var client = new AmazonS3Client(_rustFsContainer.GetAccessKey(), _rustFsContainer.GetSecretKey(), config);

        var objectRequest = new PutObjectRequest();
        objectRequest.BucketName = Guid.NewGuid().ToString("D");
        objectRequest.Key = Guid.NewGuid().ToString("D");
        objectRequest.InputStream = inputStream;

        // When
        _ = await client.PutBucketAsync(objectRequest.BucketName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.PutObjectAsync(objectRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var objectResponse = await client.GetObjectAsync(objectRequest.BucketName, objectRequest.Key, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(byte.MaxValue, objectResponse.ContentLength);
    }
}