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
      var version = await GetVersionAsync(ct)
        .ConfigureAwait(false);

      return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) > -1;
    }

    public Task<SystemInfoResponse> GetInfoAsync(CancellationToken ct = default)
    {
      return Docker.System.GetSystemInfoAsync(ct);
    }

    public Task<VersionResponse> GetVersionAsync(CancellationToken ct = default)
    {
      return Docker.System.GetVersionAsync(ct);
    }
  }
}
