namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.MessageBroker
{
  using System;
  using System.Collections.Generic;
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
      // When
      using var producer = new ProducerBuilder<string, string>(new Dictionary<string, string>()
      {
        { "bootstrap.servers", this.kafkaFixture.Container.BootstrapServers }
      }).Build();

      var productionReportTaskSrc = new TaskCompletionSource<DeliveryReport<string, string>>();

      producer.Produce("test_topic", new Message<string, string>()
      {
        Value = "TestMessage"
      }, report => productionReportTaskSrc.SetResult(report));

      await productionReportTaskSrc.Task;

      // Then
      using var consumer = new ConsumerBuilder<string, string>(new Dictionary<string, string>()
      {
        { "bootstrap.servers", this.kafkaFixture.Container.BootstrapServers },
        { "auto.offset.reset", "earliest" },
        { "group.id", "test_consumer" }
      }).Build();

      consumer.Subscribe("test_topic");

      var result = consumer.Consume(TimeSpan.FromSeconds(5));

      Assert.NotNull(result);
      Assert.Equal("TestMessage", result.Message.Value);
    }
  }
}
