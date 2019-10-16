namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;

  internal class WaitUntilOperationSucceeds : WaitUntilContainerIsRunning
  {
    private readonly int maxCallCount;
    private readonly Func<bool> operation;

    private int tryCount = 0;

    public WaitUntilOperationSucceeds(Func<bool> operation, int maxCallCount = 4)
    {
      this.maxCallCount = maxCallCount;
      this.operation = operation;
    }

    public override async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => base.Until(id));

      if (++this.tryCount > this.maxCallCount)
      {
        throw new TimeoutException($"Number of failed operations exceeded max count ({this.maxCallCount}).");
      }

      return this.operation();
    }
  }
}
