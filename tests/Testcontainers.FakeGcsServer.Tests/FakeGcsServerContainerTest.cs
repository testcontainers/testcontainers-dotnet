namespace Testcontainers.FakeGcsServer;

public sealed class FakeGcsServerContainerTest : IAsyncLifetime
{
    private readonly FakeGcsServerContainer _fakeGcsServerContainer =
        new FakeGcsServerBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _fakeGcsServerContainer.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _fakeGcsServerContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DownloadObjectReturnsUploadObject()
    {
        // Given
        const string helloWorld = "Hello, World!";

        using var writeStream = new MemoryStream(Encoding.Default.GetBytes(helloWorld));

        using var readStream = new MemoryStream();

        var project = Guid.NewGuid().ToString("D");

        var bucket = Guid.NewGuid().ToString("D");

        var fileName = Path.GetRandomFileName();

        var storageClientBuilder = new StorageClientBuilder();
        storageClientBuilder.UnauthenticatedAccess = true;
        storageClientBuilder.BaseUri = _fakeGcsServerContainer.GetConnectionString();

        // When
        var client = await storageClientBuilder
            .BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client
            .CreateBucketAsync(
                project,
                bucket,
                cancellationToken: TestContext.Current.CancellationToken
            )
            .ConfigureAwait(true);

        _ = await client
            .UploadObjectAsync(
                bucket,
                fileName,
                "text/plain",
                writeStream,
                cancellationToken: TestContext.Current.CancellationToken
            )
            .ConfigureAwait(true);

        _ = await client
            .DownloadObjectAsync(
                bucket,
                fileName,
                readStream,
                cancellationToken: TestContext.Current.CancellationToken
            )
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloWorld, Encoding.Default.GetString(readStream.ToArray()));
    }
}
