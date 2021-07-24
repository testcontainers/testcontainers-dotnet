namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Oracle.ManagedDataAccess.Client;

  public sealed class OracleFixture : ModuleFixture<OracleTestcontainer>
  {
    public OracleFixture()
      : base(new TestcontainersBuilder<OracleTestcontainer>()
        .WithDatabase(new OracleTestcontainerConfiguration())
        .Build())
    {
    }

    public Task<DbConnection> GetConnection()
    {
      return Task.FromResult<DbConnection>(new OracleConnection(this.Container.ConnectionString));
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
