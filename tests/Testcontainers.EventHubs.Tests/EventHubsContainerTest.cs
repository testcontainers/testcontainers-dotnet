namespace Testcontainers.EventHubs;

public abstract class EventHubsContainerTest : IAsyncLifetime
{
    private static readonly ConfigurationBuilder ConfigurationBuilder = ConfigurationBuilder.Create()
        .WithEventHub(EventHubName, 2, [EventHubConsumerGroupName]);
    
    private readonly EventHubsContainer _eventHubsContainer = new EventHubsBuilder()
        .WithAcceptLicenseAgreement(true)
        .WithConfigurationBuilder(ConfigurationBuilder)
        .Build();

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
    public sealed class EventHubsDefaultConfiguration : EventHubsContainerTest;
}