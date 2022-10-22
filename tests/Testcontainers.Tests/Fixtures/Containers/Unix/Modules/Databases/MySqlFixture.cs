namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using MySqlConnector;

  public abstract class MySqlBaseFixture : DatabaseFixture<MySqlTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration;

    public MySqlBaseFixture(string username, string password)
    {
      this.configuration = new MySqlTestcontainerConfiguration { Database = "db", Username = username, Password = password };
      this.Container = new TestcontainersBuilder<MySqlTestcontainer>()
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

  [UsedImplicitly]
  public sealed class MySqlNormalUserFixture : MySqlBaseFixture
  {
    public MySqlNormalUserFixture() : base("mysql", "mysql")
    {
    }
  }

  [UsedImplicitly]
  public sealed class MySqlRootUserFixture : MySqlBaseFixture
  {
    public MySqlRootUserFixture() : base("root", "root")
    {
    }
  }
}
