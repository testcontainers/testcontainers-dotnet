namespace Testcontainers.CouchDb;

public sealed class CouchDbContainerTest : IAsyncLifetime
{
    private readonly CouchDbContainer _couchDbContainer = new CouchDbBuilder().Build();

    public Task InitializeAsync()
    {
        return _couchDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _couchDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PutDatabaseReturnsHttpStatusCodeCreated()
    {
        // Given
        using var client = new MyCouchClient(_couchDbContainer.GetConnectionString(), "db");

        // When
        var database = await client.Database.PutAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.Created, database.StatusCode);
    }
}