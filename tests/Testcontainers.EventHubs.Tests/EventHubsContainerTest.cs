namespace Testcontainers.EventHubs;

public abstract class EventHubsContainerTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer;

    private EventHubsContainer _eventHubsContainer;

    private readonly INetwork _network = new NetworkBuilder().WithName(NetworkName).Build();
    
    private const string NetworkName = "eh-network";
    private const string AzuriteNetworkAlias = "azurite";
    
    private const string EventHubName = "testeventhub";
    private const string EventHubConsumerGroupName = "testconsumergroup";
    
    private EventHubsContainerTest()
    {
        _azuriteContainer = new AzuriteBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases(AzuriteNetworkAlias)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _azuriteContainer.StartAsync();
        
        var configurationBuilder = ConfigurationBuilder
            .Create()
            .WithEventHub(EventHubName, "2", new[] { EventHubConsumerGroupName });

        var builder = new EventHubsBuilder()
            .WithNetwork(_network)
            .WithConfigurationBuilder(configurationBuilder)
            .WithAzuriteBlobEndpoint(AzuriteNetworkAlias)
            .WithAzuriteTableEndpoint(AzuriteNetworkAlias);
        
        _eventHubsContainer = builder.Build();
        
        await _eventHubsContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _eventHubsContainer.DisposeAsync();
        
        await _azuriteContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SendAndReceivesEvents()
    {
        // Give
        var eventBody = Encoding.Default.GetBytes("test");
        var producerClient = new EventHubProducerClient(_eventHubsContainer.GetConnectionString(), EventHubName);
        
        // When
        var eventDataBatch = await producerClient.CreateBatchAsync();
        eventDataBatch.TryAdd(new EventData(eventBody));
        
        var thrownExceptionSend = await Record.ExceptionAsync(() => producerClient.SendAsync(eventDataBatch));

        // Then
        Assert.Null(thrownExceptionSend);
    }
    
    [UsedImplicitly]
    public sealed class EventHubsDefaultConfiguration : EventHubsContainerTest;
}