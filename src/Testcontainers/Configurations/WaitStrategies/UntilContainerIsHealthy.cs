namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilContainerIsHealthy : IWaitUntil
  {
    private readonly long failingStreak;

    public UntilContainerIsHealthy(long failingStreak)
    {
      this.failingStreak = failingStreak;
    }

    public Task<bool> Until(IContainer container, ILogger logger)
    {
      if (TestcontainersStates.Exited.Equals(container.State))
      {
        throw new TimeoutException("Container has exited.");
      }

      if (this.failingStreak < container.HealthCheckFailingStreak)
      {
        throw new TimeoutException($"Number of failed operations exceeded max count ({this.failingStreak}).");
      }

      return Task.FromResult(TestcontainersHealthStatus.Healthy.Equals(container.Health));
    }
  }
}
