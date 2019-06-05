namespace DotNet.Testcontainers.Core.Wait
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  internal class WaitUntilContainerIsRunning : IWaitUntil
  {
    public async Task<bool> Until(string id)
    {
      var container = await MetaDataClientContainers.Instance.ByIdAsync(id);
      return !container?.Status.Equals("Created") ?? false;
    }
  }
}
