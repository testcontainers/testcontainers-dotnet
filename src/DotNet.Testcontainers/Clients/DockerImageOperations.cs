namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Images.Archives;
  using DotNet.Testcontainers.Images.Configurations;

  internal sealed class DockerImageOperations : DockerApiClient, IDockerImageOperations
  {
    public DockerImageOperations(Uri endpoint) : base(endpoint)
    {
    }

    public async Task<IEnumerable<ImagesListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Images.ListImagesAsync(new ImagesListParameters { All = true }, ct)
        .ConfigureAwait(false)).ToArray();
    }

    public Task<IEnumerable<ImagesListResponse>> GetOrphanedObjects(CancellationToken ct = default)
    {
      IEnumerable<ImagesListResponse> images = Array.Empty<ImagesListResponse>();
      return Task.FromResult(images);
    }

    public async Task<ImagesListResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return (await this.GetAllAsync(ct)
        .ConfigureAwait(false)).FirstOrDefault(image => image.ID.Equals(id));
    }

    public Task<ImagesListResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return this.ByPropertyAsync("reference", name, ct);
    }

    public async Task<ImagesListResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var filters = new FilterByProperty(property, value);
      return (await this.Docker.Images.ListImagesAsync(new ImagesListParameters { All = true, Filters = filters }, ct)
        .ConfigureAwait(false)).FirstOrDefault();
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await this.ByIdAsync(id, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByNameAsync(name, ct)
        .ConfigureAwait(false) != null;
    }

    public Task CreateAsync(IDockerImage image, IAuthenticationConfiguration authConfig, CancellationToken ct = default)
    {
      return this.Docker.Images.CreateImageAsync(
        new ImagesCreateParameters
        {
          FromImage = image.FullName
        },
        new AuthConfig
        {
          ServerAddress = authConfig.RegistryEndpoint?.AbsoluteUri,
          Username = authConfig.Username,
          Password = authConfig.Password
        },
        new TraceProgress(), ct);
    }

    public Task DeleteAsync(IDockerImage image, CancellationToken ct = default)
    {
      return this.Docker.Images.DeleteImageAsync(image.FullName, new ImageDeleteParameters { Force = true }, ct);
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration config, CancellationToken ct = default)
    {
      var dockerFileArchive = new DockerfileArchive(config.DockerfileDirectory, config.Dockerfile, config.Image);

      var imageExists = await this.ExistsWithNameAsync(config.Image.FullName, ct)
        .ConfigureAwait(false);

      if (imageExists && config.DeleteIfExists)
      {
        await this.DeleteAsync(config.Image, ct)
          .ConfigureAwait(false);
      }

      using (var stream = new FileStream(dockerFileArchive.Tar(), FileMode.Open))
      {
        using (var image = await this.Docker.Images.BuildImageFromDockerfileAsync(stream, new ImageBuildParameters { Dockerfile = config.Dockerfile, Tags = new[] { config.Image.FullName } }, ct)
          .ConfigureAwait(false))
        {
          // Read the image stream to the end, to avoid disposing before Docker has done it's job.
          _ = await new StreamReader(image).ReadToEndAsync()
            .ConfigureAwait(false);
        }
      }

      return config.Image.FullName;
    }
  }
}
