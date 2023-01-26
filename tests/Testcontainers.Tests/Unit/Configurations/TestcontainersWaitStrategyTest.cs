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
      public async Task UntilImmediately()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitUntilAsync(() => this.UntilAsync(null)));
        Assert.Null(exception);
      }

      [Fact]
      public async Task WhileImmediately()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitWhileAsync(() => this.WhileAsync(null)));
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
      public async Task UntilAfter1Ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntilAsync(() => this.UntilAsync(null), 1000, 1));
      }

      [Fact]
      public async Task WhileAfter1Ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitWhileAsync(() => this.WhileAsync(null), 1000, 1));
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
      public async Task RethrowUntil()
      {
        await Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitUntilAsync(() => this.UntilAsync(null)));
      }

      [Fact]
      public async Task RethrowWhile()
      {
        await Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitWhileAsync(() => this.WhileAsync(null)));
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
