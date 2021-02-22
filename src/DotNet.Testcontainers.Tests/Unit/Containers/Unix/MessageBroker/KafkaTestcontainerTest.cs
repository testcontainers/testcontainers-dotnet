namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.MessageBroker
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading.Tasks;
  using Confluent.Kafka;
  using DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.MessageBrokers;
  using Xunit;

  public class KafkaTestcontainerTest : IClassFixture<KafkaFixture>
  {
    private readonly KafkaFixture kafkaFixture;

    public KafkaTestcontainerTest(KafkaFixture kafkaFixture)
    {
      this.kafkaFixture = kafkaFixture;
    }

    [Fact]
    public async Task StartsWorkingKafkaInstance()
    {
      // Given
      const string topic = "sample";

      var producerConfig = new ReadOnlyDictionary<string, string>(
        new Dictionary<string, string>
        {
          { "bootstrap.servers", this.kafkaFixture.Container.BootstrapServers }
        });

      var consumerConfig = new ReadOnlyDictionary<string, string>(
        new Dictionary<string, string>
        {
          { "bootstrap.servers", this.kafkaFixture.Container.BootstrapServers },
          { "auto.offset.reset", "earliest" },
          { "group.id", "sample-consumer" }
        });

      var message = new Message<string, string>
        { Value = Guid.NewGuid().ToString() };

      var deliveryReportTaskCompletionSource =
        new TaskCompletionSource<DeliveryReport<string, string>>();

      // When
      ConsumeResult<string, string> result;

      using (var producer = new ProducerBuilder<string, string>(producerConfig)
        .Build())
      {
        producer.Produce(topic, message, deliveryHandler =>
          deliveryReportTaskCompletionSource.SetResult(deliveryHandler));
        await deliveryReportTaskCompletionSource.Task;
      }

      using (var consumer = new ConsumerBuilder<string, string>(consumerConfig)
        .Build())
      {
        consumer.Subscribe(topic);
        result = consumer.Consume(TimeSpan.FromSeconds(5));
      }

      // Then
      Assert.NotNull(result);
      Assert.Equal(message.Value, result.Message.Value);
    }
  }
}
