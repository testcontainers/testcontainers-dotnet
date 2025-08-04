namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Xunit;

  public sealed class HostTcpPortWaitStrategyTest : IAsyncLifetime
  {
    private const int listeningPort = 49152;
    private const int mappedPort = 49153;
    private const int unmappedPort = 49154;
    private readonly IContainer _container = new ContainerBuilder()
      .WithImage(CommonImages.Socat)
      .WithCommand("-v")
      .WithCommand($"TCP-LISTEN:{listeningPort},crlf,reuseaddr,fork")
      .WithCommand("EXEC:cat")
      .WithPortBinding(listeningPort, true)
      .WithPortBinding(mappedPort, true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilHostTcpPortAvailable(listeningPort))
      .Build();

    public async ValueTask InitializeAsync()
    {
      await _container.StartAsync()
        .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
      return _container.DisposeAsync();
    }

    [Fact]
    public async Task HostTcpPortWaitStrategyIsSucceeded()
    {
      var hostPortWaitStrategy = new HostTcpPortWaitStrategy(listeningPort);

      var succeeded = await hostPortWaitStrategy.UntilAsync(_container);

      Assert.True(succeeded);
    }

    /// <summary>
    /// Verifies that the Wait fails when the port is mapped but not listening.
    /// This test might fail in docker configurations where mapped ports are always listened eg. DockerForMac (https://forums.docker.com/t/port-forwarding-of-exposed-ports-behaves-unexpectedly/15807/2)
    /// </summary>
    [Fact]
    public async Task HostTcpPortWaitStrategyFailsWhenPortNotListening()
    {
      var hostPortWaitStrategy = new HostTcpPortWaitStrategy(mappedPort);

      var succeeded = await hostPortWaitStrategy.UntilAsync(_container);

      Assert.False(succeeded);
    }

    [Fact]
    public async Task HostTcpPortWaitStrategyFailsWhenPortNotMapped()
    {
      var hostPortWaitStrategy = new HostTcpPortWaitStrategy(unmappedPort);

      var succeeded = await hostPortWaitStrategy.UntilAsync(_container);

      Assert.False(succeeded);
    }
  }
}
