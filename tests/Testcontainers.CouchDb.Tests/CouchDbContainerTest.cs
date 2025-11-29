namespace Testcontainers.CouchDb;

public sealed class CouchDbContainerTest : IAsyncLifetime
{
    private readonly CouchDbContainer _couchDbContainer = new CouchDbBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _couchDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _couchDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PutDatabaseReturnsHttpStatusCodeCreated()
    {
        // Given
        using var client = new MyCouchClient(_couchDbContainer.GetConnectionString(), "db");

        // When
        var database = await client.Database.PutAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.Created, database.StatusCode);
    }
}