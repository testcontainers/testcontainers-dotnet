namespace DotNet.Testcontainers.Core.Wait
{
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  internal class WaitUntilFilesExists : WaitUntilContainerIsRunning
  {
    private readonly string[] files;

    public WaitUntilFilesExists(params string[] files)
    {
      this.files = files;
    }

    public override async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => base.Until(id));

      return this.files.All(File.Exists);
    }
  }
}
