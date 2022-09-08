namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
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

    public override Task InitializeAsync()
    {
      // wait for 5 min
      return Task.WhenAny(this.Container.StartAsync(), Task.Delay(5 * 60 * 1000));
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
