namespace Testcontainers.Kafka;

public sealed class KafkaContainerTest : IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer = new KafkaBuilder().Build();

    public Task InitializeAsync()
    {
        return _kafkaContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _kafkaContainer.DisposeAsync().AsTask();
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

        var message = new Message<string, string>
        {
            Value = Guid.NewGuid().ToString("D"),
        };

        // When
        ConsumeResult<string, string> result;

        using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
        {
            _ = await producer.ProduceAsync(topic, message)
                .ConfigureAwait(false);
        }

        using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
        {
            consumer.Subscribe(topic);
            result = consumer.Consume(TimeSpan.FromSeconds(15));
        }

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);
    }
}