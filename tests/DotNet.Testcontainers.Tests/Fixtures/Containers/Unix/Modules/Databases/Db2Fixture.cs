namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Data.SqlClient;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  public sealed class Db2Fixture : DatabaseFixture<MsSqlTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new Db2TestcontainerConfiguration { Database = "TestDb", Password = "yourStrong(!)Password" }; // https://hub.docker.com/r/ibmcom/db2.

    public Db2Fixture()
    {
      this.Container = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(this.configuration)
        .WithPrivileged(true)
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
