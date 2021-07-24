namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  public sealed class KafkaFixture : ModuleFixture<KafkaTestcontainer>
  {
    public KafkaFixture()
      : base(new TestcontainersBuilder<KafkaTestcontainer>()
        .WithKafka(new KafkaTestcontainerConfiguration())
        .Build())
    {
    }

    public override Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
