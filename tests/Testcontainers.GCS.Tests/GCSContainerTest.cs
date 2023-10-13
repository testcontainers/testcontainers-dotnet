namespace Testcontainers.GCS;

public abstract class GCSContainerTest : IAsyncLifetime
{
    private readonly GCSContainer _gcsontainer = new GCSBuilder().Build();

    public Task InitializeAsync()
    {
        return _gcsontainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _gcsontainer.DisposeAsync().AsTask();
    }

    public sealed class BlobService : GCSContainerTest
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
                BaseUri = _gcsontainer.GetConnectionString()
            }.BuildAsync();
            
            // When
            var bucket = await client.CreateBucketAsync(testProject, testBucket);
            var buckets = await client.ListBucketsAsync(testProject).ToListAsync();

            // Then
            Assert.True(bucket.Name == testBucket);
            Assert.True(buckets.Count == 1);
        }
    }
    
}