namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers.Modules;

  internal class WaitUntilContainerIsRunning : IWaitUntil
  {
    public static readonly IWaitUntil WaitStrategy = new WaitUntilContainerIsRunning();

    private static readonly TestcontainersState[] ContainerHasBeenRunningStates = { TestcontainersState.Running, TestcontainersState.Exited };

    private WaitUntilContainerIsRunning()
    {
    }

    public async Task<bool> Until(Uri endpoint, string id)
    {
      var container = await new TestcontainersClient(endpoint).GetContainer(id);
      var state = (TestcontainersState)Enum.Parse(typeof(TestcontainersState), container.State, true);
      return ContainerHasBeenRunningStates.Contains(state);
    }
  }
}
