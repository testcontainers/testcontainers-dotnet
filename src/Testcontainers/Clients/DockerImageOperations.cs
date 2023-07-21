namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
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
      var images = await ParseDockerFileForImages( configuration, ct );

      foreach ( var dockerImage in images )
      {
        _logger.LogInformation("Registry Hostname: " + dockerImage.GetHostname());
        var authConfig = await GetAuthConfig(dockerImage.GetHostname(), ct);
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


    private async Task<IEnumerable<DockerImage>> ParseDockerFileForImages( IImageFromDockerfileConfiguration aConfiguration, CancellationToken ct = default )
    {
      var list = new List<DockerImage>();

      using (var streamReader = File.OpenText(
               Path.Combine(aConfiguration.DockerfileDirectory, aConfiguration.Dockerfile)))
      {
        string line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
          if (line.StartsWith("FROM"))
          {
            var imageUrl = StripDockerFileFROMLine(line);

            ParseDockerImage(imageUrl, out var repository, out var image, out var tag);

            list.Add(new DockerImage(repository, image, tag));
          }
        }
      }
      return list;
    }

    private static void ParseDockerImage(string imageUrl, out string repository, out string image, out string tag)
    {
      // Initialize the variables to empty strings
      repository = "";

      // Split the image URL by "/"
      string[] parts = imageUrl.Split('/');

      // Check if the image URL contains at least one "/" (indicating a repository)
      if (parts.Length >= 2)
      {
        // Check if the last part contains a ":" (indicating a tag)
        string lastPart = parts[parts.Length - 1];

        ParseTag(lastPart, out image, out tag);

        // determine if first part ist url or everything is part of the image
        string firstPart = parts[0];

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
        if (hostname.Contains('.') || hostname.Contains(':'))
        {
          return true;
        }
        return false;
      }
    }


    private static void ParseTag(string input, out string image, out string tag)
    {
      // Check if the input contains a ":" (indicating a tag)
      tag = "";

      int tagSeparatorIndex = input.IndexOf(':');

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

    public static string StripDockerFileFROMLine(string line)
    {
      var line_ = line.ToLower();
      line_ = line_.Replace("from", string.Empty);
      // Remove everythin after AS if exists
      var asIndex = line_.LastIndexOf("as");
      if (asIndex != -1)
      {
        line_ = line_.Substring(0, asIndex);
      }

      line_ = line_.Trim();
      return line_;
    }


    /// <inheritdoc />
    public async Task<IDockerRegistryAuthenticationConfiguration> GetAuthConfig(
      string aDockerRegistryServerAddress,
      CancellationToken ct = default)
    {
      var dockerRegistryServerAddress = aDockerRegistryServerAddress;
      _logger.LogInformation($"Hostname to resolve: {dockerRegistryServerAddress}");

      if (dockerRegistryServerAddress == null)
      {
        var info = await _systemOperations.GetInfoAsync(ct)
          .ConfigureAwait(false);

        dockerRegistryServerAddress = info.IndexServerAddress;
      }

      var authConfig = _registryAuthenticationProvider.GetAuthConfig(dockerRegistryServerAddress);
      
      return authConfig;
    }
  }
}
