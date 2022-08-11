// ReSharper disable once CheckNamespace to avoid introducing breaking changes
namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;
  using Xunit;

  public sealed class NetworkFixture : IAsyncLifetime
  {
    public IDockerNetwork Network { get; }
      = new TestcontainersNetworkBuilder()
        .WithDriver(NetworkDriver.Bridge)
        .WithName(Guid.NewGuid().ToString("D"))
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
