namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public static class TestcontainersWaitStrategyTest
  {
    public sealed class Finish : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task ImmediatelyUntil()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100)))
          .ConfigureAwait(true);

        Assert.Null(exception);
      }

      [Fact]
      public async Task ImmediatelyWhile()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100)))
          .ConfigureAwait(true);

        Assert.Null(exception);
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        return Task.FromResult(true);
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        return Task.FromResult(false);
      }
    }

    public sealed class Timeout : IWaitUntil, IWaitWhile
    {
      [Fact]
      public Task After100MsUntil()
      {
        return Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100)));
      }

      [Fact]
      public Task After100MsWhile()
      {
        return Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100)));
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        return Task.FromResult(false);
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        return Task.FromResult(true);
      }
    }

    public sealed class Rethrow : IWaitUntil, IWaitWhile
    {
      [Fact]
      public Task RethrowUntil()
      {
        return Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100)));
      }

      [Fact]
      public Task RethrowWhile()
      {
        return Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100)));
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        throw new NotImplementedException();
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        throw new NotImplementedException();
      }
    }
  }
}
