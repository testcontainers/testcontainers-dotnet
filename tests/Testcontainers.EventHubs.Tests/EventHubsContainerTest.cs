namespace Testcontainers.EventHubs;

public abstract class EventHubsContainerTest : IAsyncLifetime
{
    private const string EventHubsName = "eh-1";

    private const string EventHubsConsumerGroupName = "cg-1";

    private readonly EventHubsContainer _eventHubsContainer;

    private EventHubsContainerTest(EventHubsContainer eventHubsContainer)
    {
        _eventHubsContainer = eventHubsContainer;
    }

    // # --8<-- [start:UseEventHubsContainer]
    public Task InitializeAsync()
    {
        return _eventHubsContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _eventHubsContainer.DisposeAsync().AsTask();
    }

    private static EventHubsServiceConfiguration GetServiceConfiguration()
    {
        return EventHubsServiceConfiguration.Create().WithEntity(EventHubsName, 2, EventHubConsumerClient.DefaultConsumerGroupName, EventHubsConsumerGroupName);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SendEventDataBatchShouldNotThrowException()
    {
        // Given
        var message = Guid.NewGuid().ToString();

        var readOptions = new ReadEventOptions();
        readOptions.MaximumWaitTime = TimeSpan.FromSeconds(5);

        await using var producer = new EventHubProducerClient(_eventHubsContainer.GetConnectionString(), EventHubsName);

        await using var consumer = new EventHubConsumerClient(EventHubsConsumerGroupName, _eventHubsContainer.GetConnectionString(), EventHubsName);

        // When
        using var eventDataBatch = await producer.CreateBatchAsync()
            .ConfigureAwait(true);

        _ = eventDataBatch.TryAdd(new EventData(message));

        await producer.SendAsync(eventDataBatch)
            .ConfigureAwait(true);

        var asyncEnumerator = consumer.ReadEventsAsync(readOptions).GetAsyncEnumerator();
        _ = await asyncEnumerator.MoveNextAsync();

        // Then
        Assert.Equal(message, Encoding.UTF8.GetString(asyncEnumerator.Current.Data.Body.Span));
    }
    // # --8<-- [end:UseEventHubsContainer]

    // # --8<-- [start:CreateEventHubsContainer]
    [UsedImplicitly]
    public sealed class EventHubsDefaultAzuriteConfiguration : EventHubsContainerTest
    {
        public EventHubsDefaultAzuriteConfiguration()
            : base(new EventHubsBuilder()
                .WithAcceptLicenseAgreement(true)
                .WithConfigurationBuilder(GetServiceConfiguration())
                .Build())
        {
        }
    }
    // # --8<-- [end:CreateEventHubsContainer]

    [UsedImplicitly]
    public sealed class EventHubsCustomAzuriteConfiguration : EventHubsContainerTest, IClassFixture<DatabaseFixture>
    {
        public EventHubsCustomAzuriteConfiguration(DatabaseFixture fixture)
            : base(new EventHubsBuilder()
                .WithAcceptLicenseAgreement(true)
                .WithConfigurationBuilder(GetServiceConfiguration())
                // # --8<-- [start:ReuseExistingAzuriteContainer]
                .WithAzuriteContainer(fixture.Network, fixture.Container, DatabaseFixture.AzuriteNetworkAlias)
                // # --8<-- [end:ReuseExistingAzuriteContainer]
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class DatabaseFixture
    {
        public DatabaseFixture()
        {
            Network = new NetworkBuilder()
                .Build();

            Container = new AzuriteBuilder()
                .WithNetwork(Network)
                .WithNetworkAliases(AzuriteNetworkAlias)
                .Build();
        }

        public static string AzuriteNetworkAlias => EventHubsBuilder.AzuriteNetworkAlias;

        public INetwork Network { get; }

        public AzuriteContainer Container { get; }
    }
}