namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Data.SqlClient;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class MsSqlFixture : DatabaseFixture<MsSqlTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new MsSqlTestcontainerConfiguration { Database = "db", Password = "yourStrong(!)Password" }; // https://hub.docker.com/r/microsoft/mssql-server-linux/.

    public MsSqlFixture()
    {
      this.Container = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new SqlConnection(this.Container.ConnectionString);
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
