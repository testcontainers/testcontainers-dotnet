namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilOperationIsSucceeded : IWaitUntil
  {
    private readonly int _maxCallCount;

    private readonly Func<bool> _operation;

    private int _tryCount;

    public UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount)
    {
      _operation = operation;
      _maxCallCount = maxCallCount;
    }

    public Task<bool> UntilAsync(IContainer container)
    {
      if (++_tryCount > _maxCallCount)
      {
        throw new TimeoutException($"Number of failed operations exceeded max count ({_maxCallCount}).");
      }

      return Task.FromResult(_operation.Invoke());
    }
  }
}
