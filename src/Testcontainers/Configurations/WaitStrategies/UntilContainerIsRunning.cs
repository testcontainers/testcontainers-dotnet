namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilContainerIsRunning : IWaitUntil
  {
    private const TestcontainersStates ContainerHasBeenRunningStates = TestcontainersStates.Running | TestcontainersStates.Exited;

    public Task<bool> UntilAsync(IContainer container)
    {
      return Task.FromResult(ContainerHasBeenRunningStates.HasFlag(container.State));
    }
  }
}
