namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Xunit;

  public class OracleFixture : IAsyncLifetime
  {
    public OracleTestcontainer OracleTestcontainer { get; }

    public OracleFixture()
    {
      this.OracleTestcontainer = new TestcontainersBuilder<OracleTestcontainer>()
        .WithDatabase(new OracleTestcontainerConfiguration())
        .Build();
    }

    public Task InitializeAsync()
    {
      return this.OracleTestcontainer.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.OracleTestcontainer.DisposeAsync().AsTask();
    }
  }
}
