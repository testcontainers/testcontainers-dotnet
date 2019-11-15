namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public static class TestcontainersWaitStrategyTest
  {
    public class Finish : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task UntilImmediately()
      {
        await WaitStrategy.WaitUntil(() => this.Until(string.Empty));
      }

      [Fact]
      public async Task WhileImmediately()
      {
        await WaitStrategy.WaitWhile(() => this.While(string.Empty));
      }

      public Task<bool> Until(string id)
      {
        return Task.Run(() => true);
      }

      public Task<bool> While(string id)
      {
        return Task.Run(() => false);
      }
    }

    public class Timeout : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task UntilAfter1Ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntil(() => this.Until(string.Empty), timeout: 1));
      }

      [Fact]
      public async Task WhileAfter1Ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitWhile(() => this.While(string.Empty), timeout: 1));
      }

      public async Task<bool> Until(string id)
      {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return false;
      }

      public async Task<bool> While(string id)
      {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return true;
      }
    }

    public class Rethrow : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task RethrowUntil()
      {
        await Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitUntil(() => this.Until(string.Empty)));
      }

      [Fact]
      public async Task RethrowWhile()
      {
        await Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitWhile(() => this.While(string.Empty)));
      }

      public Task<bool> Until(string id)
      {
        throw new NotImplementedException();
      }

      public Task<bool> While(string id)
      {
        throw new NotImplementedException();
      }
    }
  }
}
