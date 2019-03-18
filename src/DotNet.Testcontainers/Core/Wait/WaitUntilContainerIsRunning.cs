namespace DotNet.Testcontainers.Core
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Wait;

  internal class WaitUntilContainerIsRunning : IWaitUntil
  {
    public async Task<bool> Until(string id)
    {
      var container = await MetaDataClientContainers.Instance.ByIdAsync(id);
      return !container?.Status.Equals("Created") ?? false;
    }
  }
}
