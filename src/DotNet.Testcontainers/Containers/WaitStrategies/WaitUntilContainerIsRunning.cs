namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Client;

  internal class WaitUntilContainerIsRunning : IWaitUntil
  {
    public virtual async Task<bool> Until(string id)
    {
      var container = await DockerApiClientContainer.Instance.ByIdAsync(id);
      return !"Created".Equals(container?.Status);
    }
  }
}
