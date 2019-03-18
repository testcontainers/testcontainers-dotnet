namespace DotNet.Testcontainers.Core.Wait
{
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilPortIsAvailable : IWaitUntil
  {
    private static readonly IWaitUntil WaitUntilContainerIsCreated = Wait.UntilContainerIsRunning();

    private readonly string[][] commands;

    public WaitUntilPortIsAvailable(int port)
    {
      this.commands = new string[][]
      {
        new string[] { "/bin/bash", "-c", $"while ! timeout 1 bash -c \"echo > /dev/tcp/localhost/{port}\"; do sleep 1; done" },
      };
    }

    public async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => { return WaitUntilContainerIsCreated.Until(id); });

      await Task.WhenAll(this.commands.Select(command => TestcontainersClient.Instance.ExecAsync(id, command)));

      return true;
    }
  }
}
