namespace Testcontainers.Azurite;

public sealed class AzuriteContainerTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder().Build();

    public Task InitializeAsync()
    {
        return _azuriteContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _azuriteContainer.DisposeAsync().AsTask();
    }
}