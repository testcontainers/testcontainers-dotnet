namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers.Modules;

  internal class WaitUntilContainerIsRunning : IWaitUntil
  {
    public static readonly IWaitUntil WaitStrategy = new WaitUntilContainerIsRunning();

    private static readonly string StateRunning = TestcontainersState.Running.ToString();

    private WaitUntilContainerIsRunning()
    {
    }

    public async Task<bool> Until(Uri endpoint, string id)
    {
      var container = await new TestcontainersClient(endpoint).GetContainer(id);
      return string.Equals(StateRunning, container?.State, StringComparison.OrdinalIgnoreCase);
    }
  }
}
