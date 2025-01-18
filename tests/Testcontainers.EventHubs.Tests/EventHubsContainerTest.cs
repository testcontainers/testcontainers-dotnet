namespace Testcontainers.EventHubs;

public abstract class EventHubsContainerTest : IAsyncLifetime
{
    private static readonly ConfigurationBuilder ConfigurationBuilder = ConfigurationBuilder.Create()
        .WithEventHub(EventHubName, 2, [EventHubConsumerGroupName]);
    
    private readonly EventHubsContainer _eventHubsContainer;

    private EventHubsContainerTest(EventHubsContainer eventHubsContainer)
    {
        _eventHubsContainer = eventHubsContainer;
    }
    
    private const string EventHubName = "eh-1";
    private const string EventHubConsumerGroupName = "testconsumergroup";

    public Task InitializeAsync() => _eventHubsContainer.StartAsync();
    public Task DisposeAsync() => _eventHubsContainer.DisposeAsync().AsTask();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    
    public async Task SendEvents()
    {
        // Given
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
    public sealed class EventHubsDefaultConfiguration() : EventHubsContainerTest(new EventHubsBuilder()
        .WithAcceptLicenseAgreement(true)
        .WithConfigurationBuilder(ConfigurationBuilder)
        .Build());
    
    [UsedImplicitly]
    public sealed class EventHubsConfigurationWithCustomAzurite() : EventHubsContainerTest(new EventHubsBuilder()
        .WithAcceptLicenseAgreement(true)
        .WithConfigurationBuilder(ConfigurationBuilder)
        .WithAzurite(AzuriteContainer, CustomAlias)
        .WithNetwork(Network)
        .Build())
    {
        private const string CustomAlias = "custom-alias";
        private static readonly INetwork Network = new NetworkBuilder().Build();
        
        private static readonly AzuriteContainer AzuriteContainer = new AzuriteBuilder()
            .WithNetwork(Network)
            .WithNetworkAliases(CustomAlias)
            .Build();
    } 
}