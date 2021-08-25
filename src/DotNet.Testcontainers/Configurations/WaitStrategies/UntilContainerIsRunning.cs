namespace DotNet.Testcontainers.Configurations
{
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilContainerIsRunning : IWaitUntil
  {
    private static readonly TestcontainersState[] ContainerHasBeenRunningStates = { TestcontainersState.Running, TestcontainersState.Exited };

    public Task<bool> Until(ITestcontainersContainer container, ILogger logger)
    {
      return Task.FromResult(ContainerHasBeenRunningStates.Contains(container.State));
    }
  }
}
