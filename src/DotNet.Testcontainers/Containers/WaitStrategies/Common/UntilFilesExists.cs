namespace DotNet.Testcontainers.Containers.WaitStrategies.Common
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Configurations;

  internal class UntilFilesExists : IWaitUntil
  {
    private readonly string file;

    public UntilFilesExists(string file)
    {
      this.file = file;
    }

    public Task<bool> Until(Uri endpoint, DockerClientAuthConfig clientAuthConfig, string id)
    {
      return Task.FromResult(File.Exists(this.file));
    }
  }
}
