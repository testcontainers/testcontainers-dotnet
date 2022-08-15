namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;
  using Xunit;

  public static class TestcontainersWaitStrategyTest
  {
    public sealed class Finish : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task UntilImmediately()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitUntil(() => this.Until(null, null)));
        Assert.Null(exception);
      }

      [Fact]
      public async Task WhileImmediately()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitWhile(() => this.While(null, null)));
        Assert.Null(exception);
      }

      public Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
      {
        return Task.FromResult(true);
      }

      public Task<bool> While(ITestcontainersContainer testcontainers, ILogger logger)
      {
        return Task.FromResult(false);
      }
    }

    public sealed class Timeout : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task UntilAfter1Ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntil(() => this.Until(null, null), 1000, 1));
      }

      [Fact]
      public async Task WhileAfter1Ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitWhile(() => this.While(null, null), 1000, 1));
      }

      public Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
      {
        return Task.FromResult(false);
      }

      public Task<bool> While(ITestcontainersContainer testcontainers, ILogger logger)
      {
        return Task.FromResult(true);
      }
    }

    public sealed class Rethrow : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task RethrowUntil()
      {
        await Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitUntil(() => this.Until(null, null)));
      }

      [Fact]
      public async Task RethrowWhile()
      {
        await Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitWhile(() => this.While(null, null)));
      }

      public Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
      {
        throw new NotImplementedException();
      }

      public Task<bool> While(ITestcontainersContainer testcontainers, ILogger logger)
      {
        throw new NotImplementedException();
      }
    }
  }
}
