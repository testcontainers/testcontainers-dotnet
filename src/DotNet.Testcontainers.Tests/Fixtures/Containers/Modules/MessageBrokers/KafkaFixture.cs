namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.MessageBrokers
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Testcontainers.Containers.Builders;
  using Testcontainers.Containers.Configurations.MessageBrokers;
  using Testcontainers.Containers.Modules.MessageBrokers;
  using Xunit;

  public class KafkaFixture : ModuleFixture<KafkaTestcontainer>, IAsyncLifetime
  {
    public KafkaFixture()
      : base(new TestcontainersBuilder<KafkaTestcontainer>()
        .WithKafka(new KafkaTestcontainerConfiguration())
        .Build())
    {
    }

    public Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
