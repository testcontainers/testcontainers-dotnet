namespace DotNet.Testcontainers.Containers.WaitStrategies.Common
{
  using System;
  using System.Threading.Tasks;
  using Configurations;

  internal class UntilOperationIsSucceeded : IWaitUntil
  {
    private readonly int maxCallCount;

    private readonly Func<bool> operation;

    private int tryCount;

    public UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount)
    {
      this.operation = operation;
      this.maxCallCount = maxCallCount;
    }

    public Task<bool> Until(Uri endpoint, DockerClientAuthConfig clientAuthConfig, string id)
    {
      if (++this.tryCount > this.maxCallCount)
      {
        throw new TimeoutException($"Number of failed operations exceeded max count ({this.maxCallCount}).");
      }

      return Task.FromResult(this.operation());
    }
  }
}
