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
    private readonly ILogger _logger;

    private readonly TraceProgress _traceProgress;

    public DockerImageOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      _logger = logger;
      _traceProgress = new TraceProgress(logger);
    }

    public async Task<IEnumerable<ImagesListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await Docker.Images.ListImagesAsync(new ImagesListParameters { All = true }, ct)
        .ConfigureAwait(false)).ToArray();
    }

    public async Task<ImagesListResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return (await GetAllAsync(ct)
        .ConfigureAwait(false)).FirstOrDefault(image => image.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public Task<ImagesListResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return ByPropertyAsync("reference", name, ct);
    }

    public async Task<ImagesListResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var filters = new FilterByProperty { { property, value } };
      return (await Docker.Images.ListImagesAsync(new ImagesListParameters { All = true, Filters = filters }, ct)
        .ConfigureAwait(false)).FirstOrDefault();
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await ByIdAsync(id, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await ByNameAsync(name, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task CreateAsync(IImage image, IDockerRegistryAuthenticationConfiguration dockerRegistryAuthConfig, CancellationToken ct = default)
    {
      var createParameters = new ImagesCreateParameters
      {
        FromImage = image.FullName.Replace(":" + image.Tag, string.Empty), // Workaround until https://github.com/dotnet/Docker.DotNet/issues/595 is fixed.
        Tag = image.Tag,
      };

      var authConfig = new AuthConfig
      {
        ServerAddress = dockerRegistryAuthConfig.RegistryEndpoint,
        Username = dockerRegistryAuthConfig.Username,
        Password = dockerRegistryAuthConfig.Password,
        IdentityToken = dockerRegistryAuthConfig.IdentityToken,
      };

      await Docker.Images.CreateImageAsync(createParameters, authConfig, _traceProgress, ct)
        .ConfigureAwait(false);

      _logger.DockerImageCreated(image);
    }

    public Task DeleteAsync(IImage image, CancellationToken ct = default)
    {
      _logger.DeleteDockerImage(image);
      return Docker.Images.DeleteImageAsync(image.FullName, new ImageDeleteParameters { Force = true }, ct);
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, ITarArchive dockerfileArchive, CancellationToken ct = default)
    {
      var image = configuration.Image;

      var imageExists = await ExistsWithNameAsync(image.FullName, ct)
        .ConfigureAwait(false);

      if (imageExists && configuration.DeleteIfExists.HasValue && configuration.DeleteIfExists.Value)
      {
        await DeleteAsync(image, ct)
          .ConfigureAwait(false);
      }

      var buildParameters = new ImageBuildParameters
      {
        Dockerfile = configuration.Dockerfile,
        Tags = new List<string> { image.FullName },
        BuildArgs = configuration.BuildArguments.ToDictionary(item => item.Key, item => item.Value),
        Labels = configuration.Labels.ToDictionary(item => item.Key, item => item.Value),
      };

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(buildParameters);
        }
      }

      var dockerfileArchiveFilePath = await dockerfileArchive.Tar(ct)
        .ConfigureAwait(false);

      try
      {
        using (var dockerfileArchiveStream = new FileStream(dockerfileArchiveFilePath, FileMode.Open, FileAccess.Read))
        {
          await Docker.Images.BuildImageFromDockerfileAsync(buildParameters, dockerfileArchiveStream, Array.Empty<AuthConfig>(), new Dictionary<string, string>(), _traceProgress, ct)
            .ConfigureAwait(false);

          var imageHasBeenCreated = await ExistsWithNameAsync(image.FullName, ct)
            .ConfigureAwait(false);

          if (!imageHasBeenCreated)
          {
            throw new InvalidOperationException($"Docker image {image.FullName} has not been created.");
          }
        }
      }
      finally
      {
        if (File.Exists(dockerfileArchiveFilePath))
        {
          File.Delete(dockerfileArchiveFilePath);
        }
      }

      _logger.DockerImageBuilt(image);
      return image.FullName;
    }
  }
}
