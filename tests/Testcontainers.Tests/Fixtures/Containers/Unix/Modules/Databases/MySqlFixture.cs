namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using MySqlConnector;

  [UsedImplicitly]
  public class MySqlFixture : DatabaseFixture<MySqlTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration;

    public MySqlFixture()
      : this(new MySqlTestcontainerConfiguration { Database = "db", Username = "mysql", Password = "mysql" })
    {
    }

    protected MySqlFixture(TestcontainerDatabaseConfiguration configuration)
    {
      this.configuration = configuration;
      this.Container = new TestcontainersBuilder<MySqlTestcontainer>()
        .WithDatabase(configuration)
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
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.configuration.Dispose();
      }
    }
  }
}
