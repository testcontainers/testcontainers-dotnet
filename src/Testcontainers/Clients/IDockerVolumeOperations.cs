namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;

  internal interface IDockerVolumeOperations : IHasListOperations<VolumeResponse, VolumeResponse>
  {
    Task<string> CreateAsync(IVolumeConfiguration configuration, CancellationToken ct = default);

    Task DeleteAsync(string name, CancellationToken ct = default);
  }
}
