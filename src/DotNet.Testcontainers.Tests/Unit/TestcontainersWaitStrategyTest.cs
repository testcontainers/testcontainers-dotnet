namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core;
  using Xunit;

  public class TestcontainersWaitStrategyTest
  {
    public class Finish : WaitStrategy
    {
      [Fact]
      public async Task WhileImmediately()
      {
        await this.WaitWhile();
      }

      [Fact]
      public async Task UntilImmediately()
      {
        await this.WaitUntil();
      }

      protected override async Task<bool> While()
      {
        return await Task.Run(() => false);
      }

      protected override async Task<bool> Until()
      {
        return await Task.Run(() => true);
      }
    }

    public class Timeout : WaitStrategy
    {
      [Fact]
      public async Task WhileAfter5ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
          await this.WaitWhile(timeout: 1);
        });
      }

      [Fact]
      public async Task UntilAfter5ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
          await this.WaitUntil(timeout: 1);
        });
      }

      protected override async Task<bool> While()
      {
        await Task.Delay(100);
        return false;
      }

      protected override async Task<bool> Until()
      {
        await Task.Delay(100);
        return true;
      }
    }
  }
}
