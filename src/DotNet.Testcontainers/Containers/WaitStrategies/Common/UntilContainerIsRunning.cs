namespace DotNet.Testcontainers.Containers.WaitStrategies.Common
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers.Modules;

  internal class UntilContainerIsRunning : IWaitUntil
  {
    private static readonly TestcontainersState[] ContainerHasBeenRunningStates = { TestcontainersState.Running, TestcontainersState.Exited };

    public async Task<bool> Until(Uri endpoint, string id)
    {
      var container = await new TestcontainersClient(endpoint).GetContainer(id);
      var state = (TestcontainersState)Enum.Parse(typeof(TestcontainersState), container.State, true);
      return ContainerHasBeenRunningStates.Contains(state);
    }
  }
}
