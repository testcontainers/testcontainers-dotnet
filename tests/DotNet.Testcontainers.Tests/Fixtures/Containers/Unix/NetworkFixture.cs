namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Network;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class NetworkFixture : IAsyncLifetime
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
