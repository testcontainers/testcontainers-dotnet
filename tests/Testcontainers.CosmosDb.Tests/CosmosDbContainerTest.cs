namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerTest : ContainerTest<CosmosDbBuilder, CosmosDbContainer>
{
    [Fact(Skip = "The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AccountPropertiesIdReturnsLocalhost()
    {
        // Given
        using var httpClient = Container.HttpClient;

        var cosmosClientOptions = new CosmosClientOptions();
        cosmosClientOptions.ConnectionMode = ConnectionMode.Gateway;
        cosmosClientOptions.HttpClientFactory = () => httpClient;

        using var cosmosClient = new CosmosClient(Container.GetConnectionString(), cosmosClientOptions);

        // When
        var accountProperties = await cosmosClient.ReadAccountAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal("localhost", accountProperties.Id);
    }
}