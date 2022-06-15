namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;

  internal interface IDockerImageOperations : IHasListOperations<ImagesListResponse>
  {
    Task CreateAsync(IDockerImage image, IDockerRegistryAuthenticationConfiguration dockerRegistryAuthConfig, CancellationToken ct = default);

    Task DeleteAsync(IDockerImage image, CancellationToken ct = default);

    Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default);
  }
}
