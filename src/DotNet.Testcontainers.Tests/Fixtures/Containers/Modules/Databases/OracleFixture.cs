namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.Databases
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Xunit;

  public class OracleFixture : ModuleFixture<OracleTestcontainer>, IAsyncLifetime
  {
    public OracleFixture()
      : base(new TestcontainersBuilder<OracleTestcontainer>()
        .WithDatabase(new OracleTestcontainerConfiguration())
        .Build())
    {
    }

    public Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
