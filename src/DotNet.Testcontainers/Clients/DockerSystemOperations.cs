namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerSystemOperations : DockerApiClient, IDockerSystemOperations
  {
    public DockerSystemOperations(Uri endpoint, ILogger logger)
      : base(endpoint)
    {
    }

    public async Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default)
    {
      return (await this.Docker.System.GetSystemInfoAsync(ct)
        .ConfigureAwait(false)).OperatingSystem.Contains("Windows");
    }
  }
}
