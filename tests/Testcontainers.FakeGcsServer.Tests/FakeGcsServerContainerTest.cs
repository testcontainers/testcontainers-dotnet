namespace Testcontainers.FakeGcsServer;

public abstract class FakeGcsServerContainerTest : IAsyncLifetime
{
    private readonly FakeGcsServerContainer _fakeGcsServerContainer = new FakeGcsServerBuilder().Build();

    public Task InitializeAsync()
    {
        return _fakeGcsServerContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _fakeGcsServerContainer.DisposeAsync().AsTask();
    }

    public sealed class BlobService : FakeGcsServerContainerTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
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
            var client = await storageClientBuilder.BuildAsync()
                .ConfigureAwait(false);

            _ = await client.CreateBucketAsync(project, bucket)
                .ConfigureAwait(false);

            _ = await client.UploadObjectAsync(bucket, fileName, "text/plain", writeStream)
                .ConfigureAwait(false);

            _ = await client.DownloadObjectAsync(bucket, fileName, readStream)
                .ConfigureAwait(false);

            // Then
            Assert.Equal(helloWorld, Encoding.Default.GetString(readStream.ToArray()));
        }
    }
}