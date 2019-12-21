namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class WaitStrategyImmediatelySucceedFixture : IWaitUntil
  {
    public Task<bool> Until(Uri endpoint, string id)
    {
      return Task.FromResult(true);
    }
  }
}
