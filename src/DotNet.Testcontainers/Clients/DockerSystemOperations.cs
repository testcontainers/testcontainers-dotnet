namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Threading.Tasks;

  internal sealed class DockerSystemOperations : DockerApiClient, IDockerSystemOperations
  {
    public DockerSystemOperations(Uri endpoint) : base(endpoint)
    {
    }

    public async Task<bool> GetIsWindowsEngineEnabled()
    {
      return (await this.Docker.System.GetSystemInfoAsync()).OperatingSystem.Contains("Windows");
    }
  }
}
