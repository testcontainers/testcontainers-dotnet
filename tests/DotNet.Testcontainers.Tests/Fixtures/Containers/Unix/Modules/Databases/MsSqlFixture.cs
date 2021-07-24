namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.Data.SqlClient;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  public sealed class MsSqlFixture : ModuleFixture<MsSqlTestcontainer>
  {
    public MsSqlFixture()
      : base(new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration
        {
          Password = "yourStrong(!)Password", // https://hub.docker.com/r/microsoft/mssql-server-linux/
        })
        .Build())
    {
    }

    public Task<DbConnection> GetConnection()
    {
      return Task.FromResult<DbConnection>(new SqlConnection(this.Container.ConnectionString));
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
