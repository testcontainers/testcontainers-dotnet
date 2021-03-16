namespace DotNet.Testcontainers.Containers.WaitStrategies.Common
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;

  internal class UntilFilesExists : IWaitUntil
  {
    private readonly string file;

    public UntilFilesExists(string file)
    {
      this.file = file;
    }

    public Task<bool> Until(IDockerClientConfiguration clientAuthConfig, string id)
    {
      return Task.FromResult(File.Exists(this.file));
    }
  }
}
