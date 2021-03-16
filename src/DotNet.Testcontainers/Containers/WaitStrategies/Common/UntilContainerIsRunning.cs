namespace DotNet.Testcontainers.Containers.WaitStrategies.Common
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using Configurations;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers.Modules;

  internal class UntilContainerIsRunning : IWaitUntil
  {
    private static readonly TestcontainersState[] ContainerHasBeenRunningStates = { TestcontainersState.Running, TestcontainersState.Exited };

    public async Task<bool> Until(IDockerClientConfiguration clientAuthConfig, string id)
    {
      using (var client = new TestcontainersClient(clientAuthConfig))
      {
        var container = await client.GetContainer(id)
          .ConfigureAwait(false);

        var state = (TestcontainersState)Enum.Parse(typeof(TestcontainersState), container.State, true);
        return ContainerHasBeenRunningStates.Contains(state);
      }
    }
  }
}
