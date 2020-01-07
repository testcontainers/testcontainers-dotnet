namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilBashCommandsAreCompleted : IWaitUntil
  {
    private readonly string[] bashCommands;

    public WaitUntilBashCommandsAreCompleted(params string[] bashCommands)
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
