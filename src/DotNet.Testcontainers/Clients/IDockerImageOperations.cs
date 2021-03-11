namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Images.Configurations;

  internal interface IDockerImageOperations : IHasListOperations<ImagesListResponse>
  {
    Task CreateAsync(IDockerImage image, IDockerRegistryAuthenticationConfiguration authConfig, CancellationToken ct = default);

    Task DeleteAsync(IDockerImage image, CancellationToken ct = default);

    Task<string> BuildAsync(IImageFromDockerfileConfiguration config, CancellationToken ct = default);
  }
}
