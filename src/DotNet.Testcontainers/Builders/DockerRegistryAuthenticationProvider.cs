namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Concurrent;
  using System.IO;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc />
  internal sealed class DockerRegistryAuthenticationProvider : IAuthenticationProvider
  {
    private const string DockerHub = "index.docker.io";

    private static readonly ConcurrentDictionary<string, IDockerRegistryAuthenticationConfiguration> Credentials = new ConcurrentDictionary<string, IDockerRegistryAuthenticationConfiguration>();

    private readonly FileInfo dockerConfigFile;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(ILogger logger)
      : this(GetDefaultDockerConfigFile(), logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="dockerConfigFile">The Docker config file path.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(string dockerConfigFile, ILogger logger)
      : this(new FileInfo(dockerConfigFile), logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="dockerConfigFile">The Docker config file path.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(FileInfo dockerConfigFile, ILogger logger)
    {
      this.dockerConfigFile = dockerConfigFile;
      this.logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable()
    {
      return true;
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      return Credentials.GetOrAdd(hostname ?? DockerHub, this.GetUncachedAuthConfig);
    }

    private static string GetDefaultDockerConfigFile()
    {
      var dockerConfigDirectory = Environment.GetEnvironmentVariable("DOCKER_CONFIG");
      return dockerConfigDirectory == null ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker", "config.json") : Path.Combine(dockerConfigDirectory, "config.json");
    }

    private IDockerRegistryAuthenticationConfiguration GetUncachedAuthConfig(string hostname)
    {
      IDockerRegistryAuthenticationConfiguration authConfig;

      if (this.dockerConfigFile.Exists)
      {
        using (var dockerConfigFileStream = new FileStream(this.dockerConfigFile.FullName, FileMode.Open, FileAccess.Read))
        {
          using (var dockerConfigDocument = JsonDocument.Parse(dockerConfigFileStream))
          {
            authConfig = new IAuthenticationProvider[] { new CredsHelperProvider(), new CredsStoreProvider(dockerConfigDocument, this.logger), new Base64Provider(dockerConfigDocument, this.logger) }
              .AsParallel()
              .Select(authenticationProvider => authenticationProvider.GetAuthConfig(hostname))
              .FirstOrDefault(authenticationProvider => authenticationProvider != null);
          }
        }
      }
      else
      {
        this.logger.DockerConfigFileNotFound(this.dockerConfigFile.FullName);
        return default(DockerRegistryAuthenticationConfiguration);
      }

      if (authConfig == null)
      {
        this.logger.DockerRegistryCredentialNotFound(hostname);
        return default(DockerRegistryAuthenticationConfiguration);
      }

      return authConfig;
    }
  }
}
