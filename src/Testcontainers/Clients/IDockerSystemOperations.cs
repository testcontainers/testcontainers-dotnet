namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  internal interface IDockerSystemOperations
  {
    Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default);

    Task<SystemInfoResponse> GetInfoAsync(CancellationToken ct = default);

    Task<VersionResponse> GetVersionAsync(CancellationToken ct = default);
  }
}
