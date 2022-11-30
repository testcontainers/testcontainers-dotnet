namespace DotNet.Testcontainers.Configurations.WaitStrategies
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilContainerIsHealthy : IWaitUntil
  {
    private readonly long failingStreak;

    /// <summary>
    /// Initializes a new instance of the <see cref="UntilContainerIsHealthy"/> class.
    /// </summary>
    /// <param name="failingStreak">Number of tolerated failed attempts before throwing a <see cref="ContainerDidNotStartException"/>.</param>
    public UntilContainerIsHealthy(long failingStreak)
    {
      this.failingStreak = failingStreak;
    }

    public Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      if (testcontainers.State == TestcontainersStates.Exited)
      {
        throw new ContainerDidNotStartException($"Container ${testcontainers.Name} has exited.");
      }

      if (testcontainers.HealthFailingStreak > this.failingStreak)
      {
        throw new ContainerDidNotStartException($"Container ${testcontainers.Name} did not report healthy status after allotted attempts.");
      }

      return Task.FromResult(testcontainers.Health == TestcontainersHealthStates.Healthy);
    }
  }
}
