namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class NetworkFixture : IAsyncLifetime
  {
    public INetwork Network { get; }
      = new NetworkBuilder()
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
