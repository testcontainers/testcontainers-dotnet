namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations.Volumes;

  internal interface IDockerVolumeOperations
  {
    Task<VolumeResponse> CreateAsync(ITestcontainersVolumeConfiguration configuration, CancellationToken ct = default);

    Task RemoveAsync(string name, CancellationToken ct = default);
  }
}
