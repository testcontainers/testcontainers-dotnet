namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilContainerIsHealthy : IWaitUntil
  {
    private readonly long failingStreak;

    public UntilContainerIsHealthy(long failingStreak)
    {
      this.failingStreak = failingStreak;
    }

    public Task<bool> UntilAsync(IContainer container)
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
