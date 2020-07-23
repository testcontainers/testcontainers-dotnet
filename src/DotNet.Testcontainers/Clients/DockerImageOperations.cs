namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
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
      return (await this.Docker.Images.ListImagesAsync(new ImagesListParameters { All = true }, ct)).ToArray();
    }

    public async Task<ImagesListResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return (await this.GetAllAsync(ct)).FirstOrDefault(image => image.ID.Equals(id));
    }

    public async Task<ImagesListResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByPropertyAsync("reference", name, ct);
    }

    public async Task<ImagesListResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var response = this.Docker.Images.ListImagesAsync(new ImagesListParameters { All = true, Filters = new FilterByProperty(property, value) }, ct);
      return (await response).FirstOrDefault();
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await this.ByIdAsync(id, ct) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByNameAsync(name, ct) != null;
    }

    public Task CreateAsync(IDockerImage image, CancellationToken ct = default)
    {
      return this.Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image.FullName }, null, new TraceProgress(), ct);
    }

    public Task DeleteAsync(IDockerImage image, CancellationToken ct = default)
    {
      return this.Docker.Images.DeleteImageAsync(image.FullName, new ImageDeleteParameters { Force = true }, ct);
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration config, CancellationToken ct = default)
    {
      var dockerFileArchive = new DockerfileArchive(config.DockerfileDirectory, config.Dockerfile, config.Image);

      var imageExists = await this.ExistsWithNameAsync(config.Image.FullName, ct);

      if (imageExists && config.DeleteIfExists)
      {
        await this.DeleteAsync(config.Image, ct);
      }

      using (var stream = new FileStream(dockerFileArchive.Tar(), FileMode.Open))
      {
        using (var image = await this.Docker.Images.BuildImageFromDockerfileAsync(stream, new ImageBuildParameters { Dockerfile = config.Dockerfile, Tags = new[] { config.Image.FullName } }, ct))
        {
          // Read the image stream to the end, to avoid disposing before Docker has done it's job.
          _ = await new StreamReader(image).ReadToEndAsync();
        }
      }

      return config.Image.FullName;
    }
  }
}
