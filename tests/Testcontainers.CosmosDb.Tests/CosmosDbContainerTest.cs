using System;
using System.Linq;

namespace Testcontainers.CosmosDb.Tests;

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
    public async Task CreateDatabaseAndContainerSuccessful()
    {
        // Given
        using var cosmosClient = new CosmosClient(
            _cosmosDbContainer.GetConnectionString(),
            new()
            {
                ConnectionMode = ConnectionMode.Gateway,
                HttpClientFactory = () => _cosmosDbContainer.HttpClient
            });


        // When
        var database = (await cosmosClient.CreateDatabaseIfNotExistsAsync("fakedb")).Database;
        await database.CreateContainerIfNotExistsAsync("fakecontainer", "/id");
        var databaseProperties = (await cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>().ReadNextAsync()).First();


        // Then
        Assert.Equal("fakedb", databaseProperties.Id);
    }


    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task RetrieveItemCreated()
    {
        // Given
        using var cosmosClient = new CosmosClient(
            _cosmosDbContainer.GetConnectionString(),
            new()
            {
                ConnectionMode = ConnectionMode.Gateway,
                HttpClientFactory = () => _cosmosDbContainer.HttpClient
            });

        var database = (await cosmosClient.CreateDatabaseIfNotExistsAsync("dbfake")).Database;
        await database.CreateContainerIfNotExistsAsync("containerfake", "/id");
        var container = database.GetContainer("containerfake");

        var id = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();


        // When
        var response = await container.CreateItemAsync(
            new FakeItem { id = id, Name = name },
            new PartitionKey(id));

        var itemResponse = await container.ReadItemAsync<FakeItem>(
            id,
            new PartitionKey(id));


        // Then
        Assert.Equal(id, itemResponse.Resource.id);
        Assert.Equal(name, itemResponse.Resource.Name);
    }


    private class FakeItem
    {
        public string id { get; set; }
        public string Name { get; set; }
    }
}
