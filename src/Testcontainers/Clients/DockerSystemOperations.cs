namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerSystemOperations : DockerApiClient, IDockerSystemOperations
  {
    public DockerSystemOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      _ = logger;
    }

    public async Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default)
    {
      return (await this.Docker.System.GetSystemInfoAsync(ct)
        .ConfigureAwait(false)).OperatingSystem.Contains("Windows");
    }

    public Task<VersionResponse> GetVersion(CancellationToken ct = default)
    {
      return this.Docker.System.GetVersionAsync(ct);
    }
  }
}
