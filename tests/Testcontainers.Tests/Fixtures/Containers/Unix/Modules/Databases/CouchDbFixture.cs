namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using MyCouch;

  [UsedImplicitly]
  public sealed class CouchDbFixture : DatabaseFixture<CouchDbTestcontainer, IMyCouchClient>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new CouchDbTestcontainerConfiguration { Database = "db", Username = "couchdb", Password = "couchdb" };

    public CouchDbFixture()
    {
      this.Container = new TestcontainersBuilder<CouchDbTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new MyCouchClient(this.Container.ConnectionString, this.Container.Database);
    }

    public override async Task DisposeAsync()
    {
      this.Connection.Dispose();

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
