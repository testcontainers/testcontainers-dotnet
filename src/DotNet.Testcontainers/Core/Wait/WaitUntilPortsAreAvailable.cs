namespace DotNet.Testcontainers.Core.Wait
{
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilPortsAreAvailable : IWaitUntil
  {
    private static readonly IWaitUntil waitUntilContainerIsCreated = Wait.UntilContainerIsRunning();

    private readonly string[][] commands;

    public WaitUntilPortsAreAvailable(params int[] ports)
    {
      this.commands = ports.Select(port => new string[] { "/bin/bash", "-c", $"while ! timeout 15 bash -c \"echo > /dev/tcp/localhost/{port}\"; do sleep 1; done" }).ToArray();
    }

    public async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => { return waitUntilContainerIsCreated.Until(id); });

      await Task.WhenAll(this.commands.Select(command => TestcontainersClient.Instance.ExecAsync(id, command)));

      return true;
    }
  }
}
