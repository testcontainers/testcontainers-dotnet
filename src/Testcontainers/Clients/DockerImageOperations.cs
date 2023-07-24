namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerImageOperations : DockerApiClient, IDockerImageOperations
  {
    private readonly ILogger _logger;

    private readonly TraceProgress _traceProgress;

    private readonly DockerRegistryAuthenticationProvider _registryAuthenticationProvider;

    private IDockerSystemOperations _systemOperations;

    public DockerImageOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger, DockerRegistryAuthenticationProvider registryAuthenticationProvider, IDockerSystemOperations systemOperations)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      _logger = logger;
      _traceProgress = new TraceProgress(logger);
      _registryAuthenticationProvider = registryAuthenticationProvider;
      _systemOperations = systemOperations;
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

      _logger.LogInformation($"{authConfig.ServerAddress}: {authConfig.IdentityToken}");

      await Docker.Images.CreateImageAsync(createParameters, authConfig, _traceProgress, ct)
        .ConfigureAwait(false);

      _logger.DockerImageCreated(image);
    }

    public Task DeleteAsync(IImage image, CancellationToken ct = default)
    {
      _logger.DeleteDockerImage(image);
      return Docker.Images.DeleteImageAsync(image.FullName, new ImageDeleteParameters { Force = true }, ct);
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      var image = configuration.Image;

      ITarArchive dockerfileArchive = new DockerfileArchive(configuration.DockerfileDirectory, configuration.Dockerfile, image, _logger);
      var images = await GetDependentImagesAsync( configuration, ct );

      foreach ( var dockerImage in images )
      {
        _logger.LogInformation("Registry Hostname: " + dockerImage.GetHostname());
        var authConfig = await GetAuthConfigAsync(dockerImage.GetHostname(), ct);
        await CreateAsync(dockerImage, authConfig, ct);
      }

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

    private async Task<IEnumerable<DockerImage>> GetDependentImagesAsync( IImageFromDockerfileConfiguration configuration, CancellationToken ct = default )
    {
      var list = new List<DockerImage>();

      using (var streamReader = File.OpenText(
               Path.Combine(configuration.DockerfileDirectory, configuration.Dockerfile)))
      {
        string line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
          if (line.StartsWith("FROM"))
          {
            var imageUrl = GetDockerImageUrl(line);

            ParseDockerImageUrl(imageUrl, out var repository, out var image, out var tag);

            list.Add(new DockerImage(repository, image, tag));
          }
        }
      }
      return list;
    }

    private static void ParseDockerImageUrl(string imageUrl, out string repository, out string image, out string tag)
    {
      repository = "";

      // Split the image URL by "/"
      var parts = imageUrl.Split('/');

      // Check if the image URL contains at least one "/" (indicating a repository)
      if (parts.Length >= 2)
      {
        // Check if the last part contains a ":" (indicating a tag)
        var lastPart = parts[parts.Length - 1];

        ParseTag(lastPart, out image, out tag);

        // determine if first part ist url or everything is part of the image
        var firstPart = parts[0];

        if (ValidateHostname(firstPart))
        {
          repository = firstPart;
          var firstPartImage = string.Join("/", parts, 1, parts.Length - 2);

          if (!string.IsNullOrEmpty(firstPartImage))
          {
            image = firstPartImage + "/" + image;
          }
        }
        else
        {
          var firstPartImage = string.Join("/", parts, 0, parts.Length - 1);
          if (!string.IsNullOrEmpty(firstPartImage))
          {
            image = firstPartImage + "/" + image;
          }
        }
      }
      // If there's no "/", consider the whole URL as the image name
      else
      {
        ParseTag(imageUrl, out image, out tag);
      }
    }

    private static bool ValidateHostname(string hostname)
    {
      try
      {
        // Try to get IP addresses for the hostname
        IPAddress[] addresses = Dns.GetHostAddresses(hostname);
        return true;
      }
      catch (Exception)
      {
        // If an exception is thrown, the hostname is not reachable in network
        // but if it contains a . or a : it might just be temporarily unreachable
        return hostname.Contains('.') || hostname.Contains(':');
      }
    }

    private static void ParseTag(string input, out string image, out string tag)
    {
      // Check if the input contains a ":" (indicating a tag)
      tag = "";

      var tagSeparatorIndex = input.IndexOf(':');

      if (tagSeparatorIndex != -1)
      {
        image = input.Substring(0, tagSeparatorIndex);
        tag = input.Substring(tagSeparatorIndex + 1);
      }
      else
      {
        image = input;
      }
    }

    private static string GetDockerImageUrl(string inputLine)
    {
      // remove everything except docker image url from line
      var line = inputLine.ToLower();
      line = line.Replace("from", string.Empty); // remove FROM at beginning from line

      var asIndex = line.LastIndexOf("as", StringComparison.Ordinal ); // remove all after AS in line
      if (asIndex != -1)
      {
        line = line.Substring(0, asIndex);
      }

      line = line.Trim();
      return line;
    }

    /// <inheritdoc />
    public async Task<IDockerRegistryAuthenticationConfiguration> GetAuthConfigAsync(
      string dockerRegistryServerAddress,
      CancellationToken ct = default)
    {
      if (dockerRegistryServerAddress == null)
      {
        var info = await _systemOperations.GetInfoAsync(ct)
          .ConfigureAwait(false);

        dockerRegistryServerAddress = info.IndexServerAddress;
      }

      var authConfig = _registryAuthenticationProvider.GetAuthConfig( dockerRegistryServerAddress );
      
      return authConfig;
    }
  }
}
