namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Azure.Cosmos;

  [UsedImplicitly]
  public sealed class CosmosDbFixture : DatabaseFixture<CosmosDbTestcontainer, CosmosClient>
  {
    private readonly CosmosDbTestcontainerConfiguration configuration = new CosmosDbTestcontainerConfiguration();

    private readonly CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

    public CosmosDbFixture()
    {
      this.Container = new TestcontainersBuilder<CosmosDbTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override Task InitializeAsync()
    {
      return this.Container.StartAsync(this.cts.Token);
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
