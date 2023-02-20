namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

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

    public Task<bool> UntilAsync(IContainer container)
    {
      if (++this.tryCount > this.maxCallCount)
      {
        throw new TimeoutException($"Number of failed operations exceeded max count ({this.maxCallCount}).");
      }

      return Task.FromResult(this.operation.Invoke());
    }
  }
}
