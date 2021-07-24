namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerImageOperations : DockerApiClient, IDockerImageOperations
  {
    private readonly ILogger logger;

    private readonly TraceProgress traceProgress;

    public DockerImageOperations(Uri endpoint, ILogger logger)
      : base(endpoint)
    {
      this.logger = logger;
      this.traceProgress = new TraceProgress(logger);
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
        .ConfigureAwait(false)).FirstOrDefault(image => image.ID.Equals(id, StringComparison.Ordinal));
    }

    public Task<ImagesListResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return this.ByPropertyAsync("reference", name, ct);
    }

    public async Task<ImagesListResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var filters = new FilterByProperty { { property, value } };
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

    public async Task CreateAsync(IDockerImage image, IDockerRegistryAuthenticationConfiguration dockerRegistryAuthConfig, CancellationToken ct = default)
    {
      var createParameters = new ImagesCreateParameters
      {
        FromImage = image.FullName,
      };

      var authConfig = new AuthConfig
      {
        ServerAddress = dockerRegistryAuthConfig.RegistryEndpoint?.AbsoluteUri,
        Username = dockerRegistryAuthConfig.Username,
        Password = dockerRegistryAuthConfig.Password,
      };

      await this.Docker.Images.CreateImageAsync(createParameters, authConfig, this.traceProgress, ct)
        .ConfigureAwait(false);

      this.logger.LogInformation("Image {fullName} created", image.FullName);
    }

    public Task DeleteAsync(IDockerImage image, CancellationToken ct = default)
    {
      this.logger.LogInformation("Deleting image {fullName}", image.FullName);
      return this.Docker.Images.DeleteImageAsync(image.FullName, new ImageDeleteParameters { Force = true }, ct);
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      var image = configuration.Image;

      ITarArchive dockerFileArchive = new DockerfileArchive(configuration.DockerfileDirectory, configuration.Dockerfile, image, this.logger);

      var imageExists = await this.ExistsWithNameAsync(image.FullName, ct)
        .ConfigureAwait(false);

      if (imageExists && configuration.DeleteIfExists)
      {
        await this.DeleteAsync(image, ct)
          .ConfigureAwait(false);
      }

      var buildParameters = new ImageBuildParameters
      {
        Dockerfile = configuration.Dockerfile,
        Tags = new[] { image.FullName },
      };

      using (var dockerFileStream = new FileStream(dockerFileArchive.Tar(), FileMode.Open))
      {
        using (var dockerImageStream = await this.Docker.Images.BuildImageFromDockerfileAsync(dockerFileStream, buildParameters, ct)
          .ConfigureAwait(false))
        {
          // Read the image stream to the end, to avoid disposing before Docker has done it's job.
          _ = await new StreamReader(dockerImageStream).ReadToEndAsync()
            .ConfigureAwait(false);
        }
      }

      this.logger.LogInformation("Image {fullName} built", image.FullName);
      return image.FullName;
    }
  }
}
