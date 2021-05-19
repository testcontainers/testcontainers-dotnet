namespace DotNet.Testcontainers.Tests.Fixtures.Networks
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Networks;
  using DotNet.Testcontainers.Networks.Builders;
  using DotNet.Testcontainers.Networks.Configurations;
  using Xunit;

  public class NetworkFixture : IAsyncLifetime
  {
    public IDockerNetwork Network { get; }
      = new TestcontainersNetworkBuilder()
        .WithDriver(NetworkDriver.Bridge)
        .WithName("test-network")
        .Build();

    public Task InitializeAsync()
    {
      return this.Network.CreateAsync();
    }

    public Task DisposeAsync()
    {
      return this.Network.DeleteAsync();
    }
  }
}
