namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core;

  public class WaitStrategyFixture : WaitStrategy
  {
    private readonly long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    protected override async Task<bool> While()
    {
      return await Task.Run(() => true);
    }

    protected override async Task<bool> Until()
    {
      return await Task.Run(() =>
      {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > this.timestamp + 5;
      });
    }
  }
}
