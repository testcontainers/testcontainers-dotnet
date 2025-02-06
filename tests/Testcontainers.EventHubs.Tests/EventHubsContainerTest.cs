namespace Testcontainers.EventHubs;

public abstract class EventHubsContainerTest : IAsyncLifetime
{
    // # --8<-- [start:MinimalConfigurationJson]
    private const string EventHubsName = "eh-1";

    private const string EventHubsConsumerGroupName = "cg-1";

    private static readonly EventHubsServiceConfiguration ConfigurationBuilder = EventHubsServiceConfiguration
        .Create()
        .WithEntity(EventHubsName, 2, [EventHubsConsumerGroupName]);
    // # --8<-- [end:MinimalConfigurationJson]

    private readonly EventHubsContainer _eventHubsContainer;

    private EventHubsContainerTest(EventHubsContainer eventHubsContainer)
    {
        _eventHubsContainer = eventHubsContainer;
    }

    // # --8<-- [start:EventHubsUsage]
    public Task InitializeAsync()
    {
        return _eventHubsContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _eventHubsContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SendEventDataBatchShouldNotThrowException()
    {
        // Given
        var message = Guid.NewGuid().ToString();

        await using var client = new EventHubProducerClient(_eventHubsContainer.GetConnectionString(), EventHubsName);

        // When
        var properties = await client.GetEventHubPropertiesAsync()
            .ConfigureAwait(true);

        using var eventDataBatch = await client.CreateBatchAsync()
            .ConfigureAwait(true);

        eventDataBatch.TryAdd(new EventData(message));

        await client.SendAsync(eventDataBatch)
            .ConfigureAwait(true);

        // Then
        Assert.NotNull(properties);
    }
    // # --8<-- [end:EventHubsUsage]
    
    // # --8<-- [start:EventHubsKafkaUsage]
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task UsingKafkaShouldNotThrowException()
    {
        // Given
        var kafkaPort = _eventHubsContainer.GetMappedPublicPort(EventHubsBuilder.KafkaPort);
        var bootstrapServer = $"localhost:{kafkaPort}";
        var connectionString = _eventHubsContainer.GetConnectionString();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServer,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "$ConnectionString",
            SaslPassword = connectionString
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServer,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "$ConnectionString",
            SaslPassword = connectionString,
            GroupId = EventHubsConsumerGroupName,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        var message = new Message<string, string>
        {
            Value = Guid.NewGuid().ToString("D"),
        };

        // When
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _ = await producer.ProduceAsync(EventHubsName, message)
            .ConfigureAwait(true);

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(EventHubsName);

        var result = consumer.Consume(TimeSpan.FromSeconds(15));

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);
    }
    
    // # --8<-- [end:EventHubsKafkaUsage]

    // # --8<-- [start:MinimalConfigurationEventHubs]
    [UsedImplicitly]
    public sealed class EventHubsDefaultAzuriteConfiguration : EventHubsContainerTest
    {
        public EventHubsDefaultAzuriteConfiguration()
            : base(new EventHubsBuilder()
                .WithAcceptLicenseAgreement(true)
                .WithConfigurationBuilder(ConfigurationBuilder)
                .Build())
        {
        }
    }
    // # --8<-- [end:MinimalConfigurationEventHubs]

    // # --8<-- [start:CustomConfigurationEventHubs]
    [UsedImplicitly]
    public sealed class EventHubsCustomAzuriteConfiguration : EventHubsContainerTest, IClassFixture<DatabaseFixture>
    {
        public EventHubsCustomAzuriteConfiguration(DatabaseFixture fixture)
            : base(new EventHubsBuilder()
                .WithAcceptLicenseAgreement(true)
                .WithConfigurationBuilder(ConfigurationBuilder)
                .WithAzuriteContainer(fixture.Network, fixture.Container, DatabaseFixture.AzuriteNetworkAlias)
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
    // # --8<-- [end:CustomConfigurationEventHubs]
}