namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;

  internal interface IDockerImageOperations : IHasListOperations<ImagesListResponse, ImageInspectResponse>
  {
    Task CreateAsync(IImage image, IDockerRegistryAuthenticationConfiguration dockerRegistryAuthConfig, CancellationToken ct = default);

    Task DeleteAsync(IImage image, CancellationToken ct = default);

    Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, ITarArchive dockerfileArchive, CancellationToken ct = default);
  }
}
