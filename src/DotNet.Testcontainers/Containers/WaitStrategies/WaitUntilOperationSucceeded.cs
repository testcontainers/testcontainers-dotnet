namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;

  internal class WaitUntilOperationSucceeded : IWaitUntil
  {
    private readonly int maxCallCount;

    private readonly Func<bool> operation;

    private readonly IWaitUntil containerIsRunningWaitStrategy;

    private int tryCount;

    public WaitUntilOperationSucceeded(Func<bool> operation, int maxCallCount) : this(operation, maxCallCount, WaitUntilContainerIsRunning.WaitStrategy)
    {
    }

    public WaitUntilOperationSucceeded(Func<bool> operation, int maxCallCount, IWaitUntil containerIsRunningWaitStrategy)
    {
      this.maxCallCount = maxCallCount;
      this.operation = operation;
      this.containerIsRunningWaitStrategy = containerIsRunningWaitStrategy;
    }

    public async Task<bool> Until(Uri endpoint, string id)
    {
      await WaitStrategy.WaitUntil(() => this.containerIsRunningWaitStrategy.Until(endpoint, id));

      if (++this.tryCount > this.maxCallCount)
      {
        throw new TimeoutException($"Number of failed operations exceeded max count ({this.maxCallCount}).");
      }

      return this.operation();
    }
  }
}
