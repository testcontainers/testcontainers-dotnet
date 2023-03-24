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
      _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("echo \"Started\" | tee /dev/stderr && trap : TERM INT; sleep infinity & wait")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Started"))
        .Build();
    }

    public Task InitializeAsync()
    {
      return _container.StartAsync(_cts.Token);
    }

    public Task DisposeAsync()
    {
      return _container.StartAsync();
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
