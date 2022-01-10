namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using IBM.Data.DB2.Core;


  public sealed class Db2Fixture : DatabaseFixture<Db2Testcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new Db2TestcontainerConfiguration { Database = "TestDb", Password = "yourStrong(!)Password" }; // https://hub.docker.com/r/ibmcom/db2.

    public Db2Fixture()
    {
      this.Container = new TestcontainersBuilder<Db2Testcontainer>()
        .WithDatabase(this.configuration)
        .WithPrivileged(true)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new DB2Connection(this.Container.ConnectionString);
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
