namespace DotNet.Testcontainers.Core.Wait
{
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilPortsAreAvailable : WaitUntilContainerIsRunning
  {
    private readonly string[][] commands;

    public WaitUntilPortsAreAvailable(params int[] ports)
    {
      this.commands = ports.Select(port => new string[] { "/bin/bash", "-c", $"while ! timeout 15 bash -c \"echo > /dev/tcp/localhost/{port}\"; do sleep 1; done" }).ToArray();
    }

    public override async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => base.Until(id));

      await Task.WhenAll(this.commands.Select(command => TestcontainersClient.Instance.ExecAsync(id, command)));

      return true;
    }
  }
}
