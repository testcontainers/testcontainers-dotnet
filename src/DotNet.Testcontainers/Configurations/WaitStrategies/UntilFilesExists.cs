namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Logging;

  internal class UntilFilesExists : IWaitUntil
  {
    private readonly string file;

    public UntilFilesExists(string file)
    {
      this.file = file;
    }

    public Task<bool> Until(Uri endpoint, string id, ILogger logger)
    {
      return Task.FromResult(File.Exists(this.file));
    }
  }
}
