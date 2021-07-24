namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using MyCouch;

  public sealed class CouchDbFixture : ModuleFixture<CouchDbTestcontainer>
  {
    public CouchDbFixture()
      : base(new TestcontainersBuilder<CouchDbTestcontainer>()
        .WithDatabase(new CouchDbTestcontainerConfiguration
        {
          Database = "db",
          Username = "couchdb",
          Password = "couchdb",
        })
        .Build())
    {
    }

    public Task<IMyCouchClient> GetClient()
    {
      return Task.FromResult<IMyCouchClient>(new MyCouchClient(this.Container.ConnectionString, this.Container.Database));
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
