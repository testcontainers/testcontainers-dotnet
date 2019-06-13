namespace DotNet.Testcontainers.Core.Wait
{
  using System.IO;
  using System.Threading.Tasks;

  internal class WaitUntilFileExists : IWaitUntil
  {
    private static readonly IWaitUntil WaitUntilContainerIsCreated = Wait.UntilContainerIsRunning();

    private readonly string file;

    public WaitUntilFileExists(string file)
    {
      this.file = file;
    }

    public async Task<bool> Until(string id)
    {
      await WaitStrategy.WaitUntil(() => { return WaitUntilContainerIsCreated.Until(id); });

      return File.Exists(this.file);
    }
  }
}
