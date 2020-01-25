namespace DotNet.Testcontainers.Containers.WaitStrategies.Common
{
  using System;
  using System.IO;
  using System.Threading.Tasks;

  internal class UntilFilesExists : IWaitUntil
  {
    private readonly string file;

    public UntilFilesExists(string file)
    {
      this.file = file;
    }

    public Task<bool> Until(Uri endpoint, string id)
    {
      return Task.FromResult(File.Exists(this.file));
    }
  }
}
