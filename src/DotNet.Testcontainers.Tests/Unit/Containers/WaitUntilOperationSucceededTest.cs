namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public class WaitUntilOperationSucceededTest
  {
    private static readonly IWaitUntil ContainerIsRunning = new WaitStrategyImmediatelySucceedFixture();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    public async Task UntilMaxRepeats(int maxCallCount, int expectedCallsCount)
    {
      // Given
      var callCounter = 0;

      // When
      await Assert.ThrowsAsync<TimeoutException>(async () =>
      {
        var wait = new WaitUntilOperationSucceeded(() =>
        {
          callCounter += 1;
          return false;
        }, maxCallCount, ContainerIsRunning);
        await WaitStrategy.WaitUntil(() => wait.Until(DockerApiEndpoint.Local, string.Empty));
      });

      // Then
      Assert.Equal(expectedCallsCount, callCounter);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(10, 1)]
    [InlineData(10, 2)]
    [InlineData(10, 5)]
    [InlineData(10, 7)]
    public async Task UntilSomeRepeats(int maxCallCount, int expectedCallsCount)
    {
      // Given
      var callCounter = 0;

      // When
      var wait = new WaitUntilOperationSucceeded(() => ++callCounter >= expectedCallsCount, maxCallCount, ContainerIsRunning);
      await WaitStrategy.WaitUntil(() => wait.Until(DockerApiEndpoint.Local, string.Empty));

      // Then
      Assert.Equal(expectedCallsCount, callCounter);
    }
  }
}
