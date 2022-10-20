namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  internal interface IDockerSystemOperations
  {
    Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default);

    Task<VersionResponse> GetVersion(CancellationToken ct = default);
  }
}
