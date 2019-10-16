namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using System.Threading.Tasks;
  using Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public class WaitUntilOperationSucceedsTest
  {
    [Theory]
    [InlineData( 1,  1)]
    [InlineData( 2,  2)]
    [InlineData( 5,  5)]
    [InlineData(10, 10)]
    [InlineData(-1,  0)]
    [InlineData( 0,  0)]
    public async Task UntilMaxRepeats(int maxCallCount, int expectedCallsCount)
    {
      var callCounter = 0;
      await Assert.ThrowsAsync<TimeoutException>(async () =>
      {
        var wait = new WaitUntilOperationSucceeds(() =>
        {
          callCounter += 1;
          return false;
        }, maxCallCount);
        await WaitStrategy.WaitUntil(() => wait.Until(string.Empty));
      });
      Assert.Equal(expectedCallsCount, callCounter);
    }

    [Theory]
    [InlineData( 1,  1)]
    [InlineData( 2,  2)]
    [InlineData( 5,  5)]
    [InlineData(10, 10)]
    [InlineData(10,  1)]
    [InlineData(10,  2)]
    [InlineData(10,  5)]
    [InlineData(10,  7)]
    public async Task UntilSomeRepeats(int maxCallCount, int expectedCallsCount)
    {
      var callCounter = 0;
      var wait = new WaitUntilOperationSucceeds(() => ++callCounter >= expectedCallsCount, maxCallCount);
      await WaitStrategy.WaitUntil(() => wait.Until(string.Empty));
      Assert.Equal(expectedCallsCount, callCounter);
    }
  }
}
