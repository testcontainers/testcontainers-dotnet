namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerTest : IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _cosmosDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _cosmosDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AccountPropertiesIdReturnsLocalhost()
    {
        // Given
        using var httpClient = _cosmosDbContainer.HttpClient;

        var cosmosClientOptions = new CosmosClientOptions();
        cosmosClientOptions.ConnectionMode = CosmosConnectionMode.Gateway;
        cosmosClientOptions.HttpClientFactory = () => httpClient;

        using var cosmosClient = new CosmosClient(_cosmosDbContainer.GetConnectionString(), cosmosClientOptions);

        // When
        var accountProperties = await cosmosClient.ReadAccountAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal("cosmosdev", accountProperties.Id);
        Assert.Equal(_cosmosDbContainer.GetConnectionString(), _cosmosDbContainer.GetConnectionString(TestcontainersConnectionMode.Host));
    }
}