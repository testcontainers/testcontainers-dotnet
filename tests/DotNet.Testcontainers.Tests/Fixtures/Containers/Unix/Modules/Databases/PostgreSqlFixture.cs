namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Npgsql;

  public sealed class PostgreSqlFixture : ModuleFixture<PostgreSqlTestcontainer>
  {
    public PostgreSqlFixture()
      : base(new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
          Database = "db",
          Username = "postgres",
          Password = "postgres",
        })
        .Build())
    {
    }

    public Task<DbConnection> GetConnection()
    {
      return Task.FromResult<DbConnection>(new NpgsqlConnection(this.Container.ConnectionString));
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
