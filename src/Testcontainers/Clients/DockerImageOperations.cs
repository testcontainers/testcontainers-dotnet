namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerImageOperations : DockerApiClient, IDockerImageOperations
  {
    private readonly TraceProgress _traceProgress;

    public DockerImageOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig, logger)
    {
      _traceProgress = new TraceProgress(logger);
    }

    public async Task<IEnumerable<ImagesListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return await DockerClient.Images.ListImagesAsync(new ImagesListParameters { All = true }, ct)
        .ConfigureAwait(false);
    }

    public async Task<IEnumerable<ImagesListResponse>> GetAllAsync(FilterByProperty filters, CancellationToken ct = default)
    {
      return await DockerClient.Images.ListImagesAsync(new ImagesListParameters { All = true, Filters = filters }, ct)
        .ConfigureAwait(false);
    }

    public async Task<ImageInspectResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return await DockerClient.Images.InspectImageAsync(id, ct)
        .ConfigureAwait(false);
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      try
      {
        _ = await ByIdAsync(id, ct)
          .ConfigureAwait(false);

        return true;
      }
      catch (DockerImageNotFoundException)
      {
        return false;
      }
    }

    public async Task CreateAsync(IImage image, IDockerRegistryAuthenticationConfiguration dockerRegistryAuthConfig, CancellationToken ct = default)
    {
      var createParameters = new ImagesCreateParameters
      {
        FromImage = image.FullName,
      };

      var authConfig = new AuthConfig
      {
        ServerAddress = dockerRegistryAuthConfig.RegistryEndpoint,
        Username = dockerRegistryAuthConfig.Username,
        Password = dockerRegistryAuthConfig.Password,
        IdentityToken = dockerRegistryAuthConfig.IdentityToken,
      };

      await DockerClient.Images.CreateImageAsync(createParameters, authConfig, _traceProgress, ct)
        .ConfigureAwait(false);

      Logger.DockerImageCreated(image);
    }

    public Task DeleteAsync(IImage image, CancellationToken ct = default)
    {
      Logger.DeleteDockerImage(image);
      return DockerClient.Images.DeleteImageAsync(image.FullName, new ImageDeleteParameters { Force = true }, ct);
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, ITarArchive dockerfileArchive, CancellationToken ct = default)
    {
      var image = configuration.Image;

      var imageExists = await ExistsWithIdAsync(image.FullName, ct)
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
          await DockerClient.Images.BuildImageFromDockerfileAsync(buildParameters, dockerfileArchiveStream, Array.Empty<AuthConfig>(), new Dictionary<string, string>(), _traceProgress, ct)
            .ConfigureAwait(false);

          var imageHasBeenCreated = await ExistsWithIdAsync(image.FullName, ct)
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

      Logger.DockerImageBuilt(image);
      return image.FullName;
    }
  }
}
