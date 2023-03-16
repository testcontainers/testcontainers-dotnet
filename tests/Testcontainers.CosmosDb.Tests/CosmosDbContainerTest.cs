using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging;

namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerTest : IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder()
        .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "2")
        .Build();

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
        var (stdout, stderr) = await _cosmosDbContainer.GetLogs()
            .ConfigureAwait(false);

        var exitCode = await _cosmosDbContainer.GetExitCode()
            .ConfigureAwait(false);

        var state = _cosmosDbContainer.State;

        var logger = TestcontainersSettings.Logger;
        logger.LogInformation(state.ToString());
        logger.LogInformation(exitCode.ToString());
        logger.LogInformation(stdout);
        logger.LogInformation(stderr);
        
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