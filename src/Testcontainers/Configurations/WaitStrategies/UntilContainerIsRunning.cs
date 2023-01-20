namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilContainerIsRunning : IWaitUntil
  {
    private const TestcontainersStates ContainerHasBeenRunningStates = TestcontainersStates.Running | TestcontainersStates.Exited;

    public Task<bool> Until(IContainer container, ILogger logger)
    {
      return Task.FromResult(ContainerHasBeenRunningStates.HasFlag(container.State));
    }
  }
}
