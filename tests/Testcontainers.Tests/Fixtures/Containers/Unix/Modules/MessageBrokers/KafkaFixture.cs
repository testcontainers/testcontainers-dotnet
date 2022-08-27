namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class KafkaFixture : IAsyncLifetime, IDisposable
  {
    private readonly KafkaTestcontainerConfiguration configuration = new KafkaTestcontainerConfiguration();

    public KafkaFixture()
    {
      this.Container = new TestcontainersBuilder<KafkaTestcontainer>()
        .WithKafka(this.configuration)
        .Build();
    }

    public KafkaTestcontainer Container { get; }

    public Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }

    public void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
