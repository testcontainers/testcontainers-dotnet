namespace Testcontainers.CouchDb;

public abstract class CouchDbContainerTest : IAsyncLifetime
{
    private readonly CouchDbContainer _couchDbContainer;

    protected CouchDbContainerTest(CouchDbContainer mongoDbContainer)
    {
        _couchDbContainer = mongoDbContainer;
    }

    public Task InitializeAsync()
    {
        return _couchDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _couchDbContainer.DisposeAsync().AsTask();
    }
}