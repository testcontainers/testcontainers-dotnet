namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  public sealed class WaitUntilFiveSecondsPassedFixture : IWaitUntil
  {
    private readonly long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    public Task<bool> Until(ITestcontainersContainer container, ILogger logger)
    {
      return Task.FromResult(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > this.timestamp + 5);
    }
  }
}
