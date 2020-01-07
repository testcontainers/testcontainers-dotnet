namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilShellCommandsAreCompleted : IWaitUntil
  {
    private readonly string[] bashCommands;

    public WaitUntilShellCommandsAreCompleted(params string[] bashCommands)
    {
      this.bashCommands = bashCommands;
    }

    public async Task<bool> Until(Uri endpoint, string id)
    {
      await WaitStrategy.WaitUntil(() => WaitUntilContainerIsRunning.WaitStrategy.Until(endpoint, id));

      var exitCodes = await Task.WhenAll(this.bashCommands.Select(command => new TestcontainersClient(endpoint).ExecAsync(id, new[] { "/bin/sh", "-c", command })).ToList());

      return exitCodes.All(exitCode => 0L.Equals(exitCode));
    }
  }
}
