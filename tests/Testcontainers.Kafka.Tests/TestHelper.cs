namespace Testcontainers.Kafka;

internal static class TestHelper
{
    public static async Task AssertKafkaProducerConsumer(KafkaContainer kafkaContainer)
    {
        // Given
        const string topic = "sample";

        var bootstrapServer = kafkaContainer.GetBootstrapAddress();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServer,
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServer,
            GroupId = "sample-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        var message = new Message<string, string>
        {
            Value = Guid.NewGuid().ToString("D"),
        };

        // When
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _ = await producer.ProduceAsync(topic, message)
            .ConfigureAwait(true);

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        var result = consumer.Consume(TimeSpan.FromSeconds(15));

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);
    }
}