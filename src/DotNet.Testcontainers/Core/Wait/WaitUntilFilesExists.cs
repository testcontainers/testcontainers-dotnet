namespace DotNet.Testcontainers.Core.Wait
{
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  internal class WaitUntilFilesExists : IWaitUntil
  {
    private static readonly IWaitUntil waitUntilContainerIsCreated = Wait.UntilContainerIsRunning();

    private readonly string[] files;

    public WaitUntilFilesExists(params string[] files)
    {
      this.files = files;
    }

    public async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => { return waitUntilContainerIsCreated.Until(id); });

      return this.files.All(File.Exists);
    }
  }
}
