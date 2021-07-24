namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilContainerIsRunning : IWaitUntil
  {
    private static readonly TestcontainersState[] ContainerHasBeenRunningStates = { TestcontainersState.Running, TestcontainersState.Exited };

    public async Task<bool> Until(Uri endpoint, string id, ILogger logger)
    {
      var client = new TestcontainersClient(endpoint, logger);

      var container = await client.GetContainer(id)
        .ConfigureAwait(false);

      var state = (TestcontainersState)Enum.Parse(typeof(TestcontainersState), container.State, true);
      return ContainerHasBeenRunningStates.Contains(state);
    }
  }
}
