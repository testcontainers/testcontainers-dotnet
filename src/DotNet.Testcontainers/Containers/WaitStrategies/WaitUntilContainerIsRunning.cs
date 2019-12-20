namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilContainerIsRunning : IWaitUntil
  {
    public static readonly IWaitUntil WaitStrategy = new WaitUntilContainerIsRunning();

    private WaitUntilContainerIsRunning()
    {
    }

    public async Task<bool> Until(Uri endpoint, string id)
    {
      var container = await new TestcontainersClient(endpoint).GetContainer(id);
      return !"Created".Equals(container?.Status);
    }
  }
}
