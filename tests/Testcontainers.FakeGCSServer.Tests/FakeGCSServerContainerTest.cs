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
            
            // Give
            var client = await new StorageClientBuilder
            {
                UnauthenticatedAccess = true,
                BaseUri = _fakeGCSServerContainer.GetConnectionString()
            }.BuildAsync();
            
            // When
            var bucket = await client.CreateBucketAsync(testProject, testBucket);

            // Then
            Assert.True(bucket.Name == testBucket);
        }
    }
    
}