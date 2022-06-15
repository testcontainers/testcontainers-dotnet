namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;

  internal interface IDockerSystemOperations
  {
    Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default);
  }
}
