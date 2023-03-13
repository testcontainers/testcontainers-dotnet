using System;
using Microsoft.Azure.Cosmos;

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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConnectionStateReturnsOpen()
    {
        // Given
        var options = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            HttpClientFactory = () => _cosmosDbContainer.HttpClient,
            RequestTimeout = TimeSpan.FromMinutes(3)
        };
        
        var client = new CosmosClient(_cosmosDbContainer.GetConnectionString(), options);
        
        // When
        var account = await client.ReadAccountAsync();
        
        // Then
        Assert.Equal("localhost", account.Id);
    }
}