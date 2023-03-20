namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerTest : IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder().Build();

    public Task InitializeAsync()
    {
        return _cosmosDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _cosmosDbContainer.DisposeAsync().AsTask();
    }

    [SkipOnLinuxEngine] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public async Task ReadAccountIdReturnsLocalhost()
    {
        // Given
        using var httpClient = _cosmosDbContainer.HttpClient;

        var cosmosClientOptions = new CosmosClientOptions();
        cosmosClientOptions.ConnectionMode = ConnectionMode.Gateway;
        cosmosClientOptions.HttpClientFactory = () => httpClient;

        using var cosmosClient = new CosmosClient(_cosmosDbContainer.GetConnectionString(), cosmosClientOptions);

        // When
        var accountProperties = await cosmosClient.ReadAccountAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal("localhost", accountProperties.Id);
    }
}