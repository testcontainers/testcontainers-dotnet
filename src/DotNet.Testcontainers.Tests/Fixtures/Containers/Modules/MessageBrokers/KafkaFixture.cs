namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.MessageBrokers
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.MessageBrokers;
  using DotNet.Testcontainers.Containers.Modules.MessageBrokers;
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
