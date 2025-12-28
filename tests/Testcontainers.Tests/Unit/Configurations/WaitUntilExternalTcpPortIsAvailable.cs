namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class WaitUntilExternalTcpPortIsAvailable : IAsyncLifetime
  {
    private const ushort ListeningPort = 49152;

    private const ushort MappedPort = 49153;

    private const ushort UnmappedPort = 49154;

    private readonly IContainer _container = new ContainerBuilder(CommonImages.Socat)
      .WithCommand("-v")
      .WithCommand($"TCP-LISTEN:{ListeningPort},crlf,reuseaddr,fork")
      .WithCommand("EXEC:cat")
      .WithPortBinding(ListeningPort, true)
      .WithPortBinding(MappedPort, true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilExternalTcpPortIsAvailable(ListeningPort))
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
    public async Task SucceedsWhenPortIsMappedAndListening()
    {
      // Given
      var waitStrategy = new UntilExternalTcpPortIsAvailable(ListeningPort);

      // When
      var success = await waitStrategy.UntilAsync(_container)
        .ConfigureAwait(true);

      // Then
      Assert.True(success);
    }

    [Fact]
    public async Task SucceedsWhenPortIsMappedButNotListening()
    {
      // Given
      var waitStrategy = new UntilExternalTcpPortIsAvailable(MappedPort);

      // When
      var success = await waitStrategy.UntilAsync(_container)
        .ConfigureAwait(true);

      // Then
      Assert.True(success);
    }

    [Fact]
    public Task ThrowsWhenPortIsNotMapped()
    {
      var waitStrategy = new UntilExternalTcpPortIsAvailable(UnmappedPort);
      return Assert.ThrowsAsync<InvalidOperationException>(() => waitStrategy.UntilAsync(_container));
    }
  }
}
