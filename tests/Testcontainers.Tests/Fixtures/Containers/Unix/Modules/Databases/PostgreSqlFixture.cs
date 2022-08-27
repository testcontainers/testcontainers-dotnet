namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Npgsql;

  [UsedImplicitly]
  public sealed class PostgreSqlFixture : DatabaseFixture<PostgreSqlTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new PostgreSqlTestcontainerConfiguration { Database = "db", Username = "postgres", Password = "postgres" };

    public PostgreSqlFixture()
    {
      this.Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new NpgsqlConnection(this.Container.ConnectionString);
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
