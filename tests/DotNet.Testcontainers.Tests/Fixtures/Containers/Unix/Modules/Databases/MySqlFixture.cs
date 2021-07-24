namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using MySql.Data.MySqlClient;

  public sealed class MySqlFixture : ModuleFixture<MySqlTestcontainer>
  {
    public MySqlFixture()
      : base(new TestcontainersBuilder<MySqlTestcontainer>()
        .WithDatabase(new MySqlTestcontainerConfiguration
        {
          Database = "db",
          Username = "mysql",
          Password = "mysql",
        })
        .Build())
    {
    }

    public Task<DbConnection> GetConnection()
    {
      return Task.FromResult<DbConnection>(new MySqlConnection(this.Container.ConnectionString));
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
