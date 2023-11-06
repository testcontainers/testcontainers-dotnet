namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class WaitUntilFileExistsInContainerTest : IAsyncLifetime, IDisposable
  {
    private const string ContainerFilePath = "/tmp/hostname";

    private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

    private readonly IContainer _container = new ContainerBuilder()
      .WithImage(CommonImages.Alpine)
      .WithEntrypoint("/bin/sh", "-c")
      .WithCommand("hostname > " + ContainerFilePath + "; trap : TERM INT; sleep infinity & wait")
      .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists(ContainerFilePath, FileSystem.Container))
      .Build();

    public Task InitializeAsync()
    {
      return _container.StartAsync(_cts.Token);
    }

    public Task DisposeAsync()
    {
      return _container.DisposeAsync().AsTask();
    }

    public void Dispose()
    {
      _cts.Dispose();
    }

    [Fact]
    public async Task ContainerIsRunning()
    {
      var execResult = await _container.ExecAsync(new List<string> { "test", "-f", ContainerFilePath })
        .ConfigureAwait(false);

      Assert.Equal(0, execResult.ExitCode);
    }
  }
}
