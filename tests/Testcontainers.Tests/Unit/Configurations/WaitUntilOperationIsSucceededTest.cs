namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public sealed class WaitUntilOperationIsSucceededTest
  {
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    public async Task UntilMaxRepeats(int maxTryCount, int expectedCount)
    {
      // Given
      var tryCount = 0;

      // When
      var wait = Wait.ForUnixContainer().UntilOperationIsSucceeded(() => ++tryCount < 0, maxTryCount).Build().Skip(1).Single();

      // Then
      await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntilAsync(() => wait.UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromSeconds(5)))
        .ConfigureAwait(true);

      Assert.Equal(expectedCount, tryCount);
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
    public async Task UntilSomeRepeats(int maxTryCount, int expectedCount)
    {
      // Given
      var tryCount = 0;

      // When
      var wait = Wait.ForUnixContainer().UntilOperationIsSucceeded(() => ++tryCount >= expectedCount, maxTryCount).Build().Skip(1).Single();

      // Then
      _ = await Record.ExceptionAsync(() => WaitStrategy.WaitUntilAsync(() => wait.UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromSeconds(5)))
        .ConfigureAwait(true);

      Assert.Equal(expectedCount, tryCount);
    }
  }
}
