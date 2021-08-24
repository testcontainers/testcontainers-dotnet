namespace DotNet.Testcontainers.Configurations
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilFilesExists : IWaitUntil
  {
    private readonly string file;

    public UntilFilesExists(string file)
    {
      this.file = file;
    }

    public Task<bool> Until(ITestcontainersContainer container, ILogger logger)
    {
      return Task.FromResult(File.Exists(this.file));
    }
  }
}
