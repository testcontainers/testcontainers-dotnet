namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using MySqlConnector;

  [UsedImplicitly]
  public sealed class MariaDbFixture : DatabaseFixture<MariaDbTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new MariaDbTestcontainerConfiguration { Database = "db", Username = "mysql", Password = "mysql" };

    public MariaDbFixture()
    {
      this.Container = new TestcontainersBuilder<MariaDbTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new MySqlConnection(this.Container.ConnectionString);
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
