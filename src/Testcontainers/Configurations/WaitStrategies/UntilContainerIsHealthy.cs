namespace DotNet.Testcontainers.Configurations.WaitStrategies
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilContainerIsHealthy : IWaitUntil
  {
    private readonly int failingStreak;

    /// <summary>
    /// Initializes a new instance of the <see cref="UntilContainerIsHealthy"/> class.
    /// </summary>
    /// <param name="failingStreak">Number of tolerated failed attempts before throwing a <see cref="ContainerDidNotStartException"/>.</param>
    public UntilContainerIsHealthy(int failingStreak)
    {
      this.failingStreak = failingStreak;
    }

    public Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      var health = testcontainers.Health;
      if (testcontainers.State != TestcontainersStates.Running || health == null)
      {
        return Task.FromResult(false);
      }

      if (health.FailingStreak > this.failingStreak)
      {
        throw new ContainerDidNotStartException($"Container ${testcontainers.Name} did not report healty status after allotted attempts.");
      }

      return Task.FromResult(health.Status == "healthy");
    }
  }
}
