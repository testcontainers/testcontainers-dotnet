namespace Testcontainers.Kafka;

public abstract class KafkaContainerTest : IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer;

    private KafkaContainerTest(KafkaContainer kafkaContainer)
    {
        _kafkaContainer = kafkaContainer;
    }

    public async ValueTask InitializeAsync()
    {
        await _kafkaContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConsumerReturnsProducerMessage()
    {
        // Given
        const string topic = "sample";

        var bootstrapServer = _kafkaContainer.GetBootstrapAddress();

        var producerConfig = new ProducerConfig();
        producerConfig.BootstrapServers = bootstrapServer;

        var consumerConfig = new ConsumerConfig();
        consumerConfig.BootstrapServers = bootstrapServer;
        consumerConfig.GroupId = "sample-consumer";
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;

        var message = new Message<string, string>();
        message.Value = Guid.NewGuid().ToString("D");

        // When
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _ = await producer.ProduceAsync(topic, message, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        var result = consumer.Consume(TimeSpan.FromSeconds(15));

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);
        Assert.Equal(_kafkaContainer.GetBootstrapAddress(), _kafkaContainer.GetConnectionString());
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _kafkaContainer.DisposeAsync();
    }

    [UsedImplicitly]
    public sealed class KafkaDefaultConfiguration : KafkaContainerTest
    {
        public KafkaDefaultConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile())
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class KafkaKRaftConfiguration : KafkaContainerTest
    {
        public KafkaKRaftConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile())
                .WithKRaft()
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class KafkaZooKeeperConfiguration : KafkaContainerTest
    {
        public KafkaZooKeeperConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile())
                .WithZooKeeper()
                .Build())
        {
        }
    }

    // Confluent Platform 8.x runs on Kafka 4.x, which rejects empty elements in
    // `advertised.listeners`. A trailing comma made the container exit on startup:
    // https://github.com/testcontainers/testcontainers-dotnet/issues/1771.
    [UsedImplicitly]
    public sealed class ConfluentKafkaV8Configuration : KafkaContainerTest
    {
        public ConfluentKafkaV8Configuration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile(stage: "confluent-v8_2_4"))
                .WithKRaft()
                .Build())
        {
        }
    }

    // Confluent Platform 8.x removed ZooKeeper, so the default consensus protocol
    // must be KRaft: https://github.com/testcontainers/testcontainers-dotnet/issues/1773.
    [UsedImplicitly]
    public sealed class ConfluentKafkaV8DefaultConfiguration : KafkaContainerTest
    {
        public ConfluentKafkaV8DefaultConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile(stage: "confluent-v8_2_4"))
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ApacheKafkaConfiguration : KafkaContainerTest
    {
        public ApacheKafkaConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile(stage: "apache-v4_1_1"))
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ApacheKafkaNativeConfiguration : KafkaContainerTest
    {
        public ApacheKafkaNativeConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile(stage: "apache-native-v4_1_1"))
                .Build())
        {
        }
    }
}