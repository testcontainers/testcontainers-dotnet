namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Xunit;

  public sealed class WaitUntilFileExistsInContainerTest : IDisposable
  {
    private readonly CancellationTokenSource _cts = new(TimeSpan.FromMinutes(1));

    public void Dispose()
    {
      _cts.Dispose();
    }

    [Fact]
    public async Task ContainerIsRunning()
    {
      // Given
      const string target = "tmp";

      const string file = "hostname";

      await using var container = new ContainerBuilder()
        .WithImage(CommonImages.Nginx)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand($"hostname > /{target}/{file}; trap : TERM INT; sleep infinity & wait")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists($"/{target}/{file}", FileSystem.Container))
        .Build();

      // When
      await container.StartAsync(_cts.Token)
        .ConfigureAwait(false);

      // Then
      var catResult = await container.ExecAsync(new List<string> { "cat", $"/{target}/{file}" });
      Assert.NotEmpty(catResult.Stdout);
    }
  }
}
