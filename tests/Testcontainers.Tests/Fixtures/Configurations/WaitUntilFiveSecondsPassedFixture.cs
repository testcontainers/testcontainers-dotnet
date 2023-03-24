namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  public sealed class WaitUntilFiveSecondsPassedFixture : IWaitUntil
  {
    private readonly long _timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    public Task<bool> UntilAsync(IContainer container)
    {
      return Task.FromResult(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > _timestamp + 5);
    }
  }
}
