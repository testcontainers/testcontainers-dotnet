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
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _kafkaContainer.DisposeAsync();
    }

    [UsedImplicitly]
    public sealed class KafkaDefaultConfiguration : KafkaContainerTest
    {
        public KafkaDefaultConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile()).Build())
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

    [UsedImplicitly]
    public sealed class ApacheKafkaConfiguration : KafkaContainerTest
    {
        public ApacheKafkaConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile())
                .WithImage("apache/kafka:3.9.1")
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ApacheKafkaNativeConfiguration : KafkaContainerTest
    {
        public ApacheKafkaNativeConfiguration()
            : base(new KafkaBuilder(TestSession.GetImageFromDockerfile())
                .WithImage("apache/kafka-native:3.9.1")
                .Build())
        {
        }
    }
}