namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Oracle.ManagedDataAccess.Client;

  [UsedImplicitly]
  public sealed class OracleFixture : DatabaseFixture<OracleTestcontainer, DbConnection>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new OracleTestcontainerConfiguration { Password = "oracle" };

    public OracleFixture()
    {
      this.Container = new TestcontainersBuilder<OracleTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new OracleConnection(this.Container.ConnectionString);
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
