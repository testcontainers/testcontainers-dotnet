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
    private readonly CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

    private readonly IContainer container;

    public WaitUntilMessageIsLoggedTest()
    {
      this.container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("echo \"Started\" | tee /dev/stderr && trap : TERM INT; sleep infinity & wait")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Started"))
        .Build();
    }

    public Task InitializeAsync()
    {
      return this.container.StartAsync(this.cts.Token);
    }

    public Task DisposeAsync()
    {
      return this.container.StartAsync();
    }

    public void Dispose()
    {
      this.cts.Dispose();
    }

    [Fact]
    public void ContainerIsRunning()
    {
      Assert.Equal(TestcontainersStates.Running, this.container.State);
    }
  }
}
