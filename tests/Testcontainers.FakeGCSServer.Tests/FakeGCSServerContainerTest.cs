namespace Testcontainers.FakeGCSServer;

public abstract class FakeGCSServerContainerTest : IAsyncLifetime
{
    private readonly FakeGCSServerContainer _fakeGCSServerContainer = new FakeGCSServerBuilder().Build();

    public Task InitializeAsync()
    {
        return _fakeGCSServerContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _fakeGCSServerContainer.DisposeAsync().AsTask();
    }

    public sealed class BlobService : FakeGCSServerContainerTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            string testProject = "test-project";
            string testBucket = "test-bucket";
            string content = "Hello Google Storage"'
            
            // Give
            var client = await new StorageClientBuilder
            {
                UnauthenticatedAccess = true,
                BaseUri = _fakeGCSServerContainer.GetConnectionString()
            }.BuildAsync();
            
            // When
            client.CreateBucket(testProject, testBucket);
            client.UploadObject(testBucket, "hello.txt", "text/plain", new MemoryStream(Encoding.UTF8.GetBytes(content)));
            var ms = new MemoryStream();
            client.DownloadObject(testBucket, "hello.txt", ms);

            var blobContent = Encoding.UTF8.GetString(ms.ToArray());



            // Then
            Assert.True(content == blobContent);
        }
    }
}