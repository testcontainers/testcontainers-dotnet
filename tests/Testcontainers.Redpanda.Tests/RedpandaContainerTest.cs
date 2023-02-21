namespace Testcontainers.Redpanda;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public sealed class RedpandaContainerTest : IAsyncLifetime
{
  private readonly RedpandaContainer _redpandaContainer = new RedpandaBuilder().Build();

  public Task InitializeAsync()
  {
    return this._redpandaContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return this._redpandaContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task consumeMessage()
  {
    const string topic = "sample";

    var producerConfig = new ReadOnlyDictionary<string, string>(
      new Dictionary<string, string>
      {
        { "bootstrap.servers", this._redpandaContainer.GetBootstrapServers() },
      });

    var consumerConfig = new ReadOnlyDictionary<string, string>(
      new Dictionary<string, string>
      {
        { "bootstrap.servers", this._redpandaContainer.GetBootstrapServers() },
        { "auto.offset.reset", "earliest" },
        { "group.id", "sample-consumer" },
      });

    var message = new Message<string, string>
    {
      Value = Guid.NewGuid().ToString(),
    };

    var deliveryReportTaskCompletionSource = new TaskCompletionSource<DeliveryReport<string, string>>();

    ConsumeResult<string, string> result;

    using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
    {
      producer.Produce(topic, message, deliveryHandler => deliveryReportTaskCompletionSource.SetResult(deliveryHandler));
      await deliveryReportTaskCompletionSource.Task;
    }

    using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
    {
      consumer.Subscribe(topic);
      result = consumer.Consume(TimeSpan.FromSeconds(5));
    }

    Assert.NotNull(result);
    Assert.Equal(message.Value, result.Message.Value);
  }

}
