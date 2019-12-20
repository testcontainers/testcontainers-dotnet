namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class WaitStrategyFixture : IWaitUntil
  {
    private readonly long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    public Task<bool> Until(Uri endpoint, string id)
    {
      return Task.Run(() => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > this.timestamp + 5);
    }
  }
}
