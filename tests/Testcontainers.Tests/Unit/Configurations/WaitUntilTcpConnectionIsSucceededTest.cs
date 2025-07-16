namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class WaitUntilTcpConnectionIsSucceededTest : IAsyncLifetime
  {
    private const int listeningPort = 49152;
    private const int mappedPort = 49153;
    private IContainer _container = new ContainerBuilder()
      .WithImage(CommonImages.Socat)
      .WithCommand("-v")
      .WithCommand($"TCP-LISTEN:{listeningPort},crlf,reuseaddr,fork")
      .WithCommand("EXEC:cat")
      .WithPortBinding(listeningPort, true)
      .WithPortBinding(mappedPort, true)
      .WithWaitStrategy(Wait.ForUnixContainer())
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
    public async Task TcpWaitStrategyUntilConnectionIsSucceeded()
    {
      var tcpWaitStrategy = new UntilTcpConnected(listeningPort);

      var succeeded = await tcpWaitStrategy.UntilAsync(_container);

      Assert.True(succeeded);
    }

    [Fact]
    public async Task TcpWaitStrategyUntilConnectionIsFailed()
    {
      var tcpWaitStrategy = new UntilTcpConnected(mappedPort);

      var succeeded = await tcpWaitStrategy.UntilAsync(_container);

      Assert.False(succeeded);
    }

    [Fact]
    public async Task TcpWaitStrategyUntilConnectionFailsWhenPortNotMapped()
    {
      const int unmappedPort = 49153;
      var tcpWaitStrategy = new UntilTcpConnected(unmappedPort);

      var succeeded = await tcpWaitStrategy.UntilAsync(_container);

      Assert.False(succeeded);
    }

  }
}
