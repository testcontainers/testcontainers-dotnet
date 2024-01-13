namespace Testcontainers.Minio;

public sealed class MinioContainerTest : IAsyncLifetime
{
    private readonly MinioContainer _minioContainer = new MinioBuilder().Build();

    public Task InitializeAsync()
    {
        return _minioContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _minioContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListBucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonS3Config();
        config.ServiceURL = _minioContainer.GetConnectionString();

        using var client = new AmazonS3Client(_minioContainer.GetAccessKey(), _minioContainer.GetSecretKey(), config);

        // When
        var buckets = await client.ListBucketsAsync()
            .ConfigureAwait(true);

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
        config.ServiceURL = _minioContainer.GetConnectionString();

        using var client = new AmazonS3Client(_minioContainer.GetAccessKey(), _minioContainer.GetSecretKey(), config);

        var objectRequest = new PutObjectRequest();
        objectRequest.BucketName = Guid.NewGuid().ToString("D");
        objectRequest.Key = Guid.NewGuid().ToString("D");
        objectRequest.InputStream = inputStream;

        // When
        _ = await client.PutBucketAsync(objectRequest.BucketName)
            .ConfigureAwait(true);

        _ = await client.PutObjectAsync(objectRequest)
            .ConfigureAwait(true);

        var objectResponse = await client.GetObjectAsync(objectRequest.BucketName, objectRequest.Key)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(byte.MaxValue, objectResponse.ContentLength);
    }
}