namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Data.Common;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  public class CosmosDbFixture : DatabaseFixture<CosmosDbTestcontainer, DbConnection>
  {
    public CosmosDbFixture()
      : this(new CosmosDbTestcontainerConfiguration())
    {
    }

    private CosmosDbFixture(CosmosDbTestcontainerConfiguration configuration)
    {
      this.Configuration = configuration;
      this.Container = new TestcontainersBuilder<CosmosDbTestcontainer>()
        .WithCosmosDb(configuration)
        .Build();
    }

    public CosmosDbTestcontainerConfiguration Configuration { get; set; }

    public override async Task InitializeAsync()
    {
      // workaround for broken cosmosdb emulator
      var maxWait = TimeSpan.FromSeconds(5 * 1000);
      var cancellationTokenSource = new CancellationTokenSource();
      var containerTask = this.Container.StartAsync(cancellationTokenSource.Token);
      var task = await Task.WhenAny(new[] { containerTask, Task.Delay(maxWait) });
      if (task != containerTask)
      {
        cancellationTokenSource.Cancel();
      }
    }

    public override async Task DisposeAsync()
    {
      if (this.Connection != null && this.Connection.State != System.Data.ConnectionState.Closed)
      {
        this.Connection.Dispose();
      }

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.Configuration.Dispose();
    }
  }
}
