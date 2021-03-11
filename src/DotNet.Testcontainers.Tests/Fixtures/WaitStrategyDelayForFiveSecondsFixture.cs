namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using Testcontainers.Containers.Configurations;

  public class WaitStrategyDelayForFiveSecondsFixture : IWaitUntil
  {
    private readonly long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    public Task<bool> Until(Uri endpoint, DockerClientAuthConfig clientAuthConfig, string id)
    {
      return Task.FromResult(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > this.timestamp + 5);
    }
  }
}
