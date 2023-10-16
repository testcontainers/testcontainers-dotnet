using System.Text;
using System.IO;

namespace Testcontainers.FakeGcsServer;

public abstract class FakeGcsServerContainerTest : IAsyncLifetime
{
    private readonly FakeGcsServerContainer _FakeGcsServerContainer = new FakeGcsServerBuilder().Build();

    public Task InitializeAsync()
    {
        return _FakeGcsServerContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _FakeGcsServerContainer.DisposeAsync().AsTask();
    }

    public sealed class BlobService : FakeGcsServerContainerTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            string testProject = "test-project";
            string testBucket = "test-bucket";
            string content = "Hello Google Storage";
            
            // Give
            var client = await new StorageClientBuilder
            {
                UnauthenticatedAccess = true,
                BaseUri = _FakeGcsServerContainer.GetConnectionString()
            }.BuildAsync();
            
            // When
            client.CreateBucket(testProject, testBucket);
            client.UploadObject(testBucket, "hello.txt", "text/plain", new MemoryStream(Encoding.UTF8.GetBytes(content)));
            using var ms = new MemoryStream();
            client.DownloadObject(testBucket, "hello.txt", ms);
            var blobContent = Encoding.UTF8.GetString(ms.ToArray());

            // Then
            Assert.Equal(content, blobContent);
        }
    }
}