namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class WaitUntilMessageIsLoggedTest : IAsyncLifetime, IDisposable
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

    private readonly IContainer _container;

    public WaitUntilMessageIsLoggedTest()
    {
      _container = new ContainerBuilder(CommonImages.Alpine)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("echo \"Started\" | tee /dev/stderr && trap : TERM INT; sleep infinity & wait")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Started"))
        .Build();
    }

    public async ValueTask InitializeAsync()
    {
      await _container.StartAsync(_cts.Token)
        .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
      return _container.DisposeAsync();
    }

    public void Dispose()
    {
      _cts.Dispose();
    }

    [Fact]
    public void ContainerIsRunning()
    {
      Assert.Equal(TestcontainersStates.Running, _container.State);
    }
  }
}
