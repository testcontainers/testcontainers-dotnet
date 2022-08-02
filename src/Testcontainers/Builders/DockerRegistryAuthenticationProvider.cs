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

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class DockerRegistryAuthenticationProvider : IDockerRegistryAuthenticationProvider
  {
    private const string DockerHub = "index.docker.io";

    private static readonly ConcurrentDictionary<string, Lazy<IDockerRegistryAuthenticationConfiguration>> Credentials = new ConcurrentDictionary<string, Lazy<IDockerRegistryAuthenticationConfiguration>>();

    private static readonly string UserProfileDockerConfigDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker");

    private readonly FileInfo dockerConfigFile;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(ILogger logger)
      : this(GetDefaultDockerConfigFilePath(), logger)
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
    public bool IsApplicable(string hostname)
    {
      return true;
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      var lazyAuthConfig = Credentials.GetOrAdd(hostname ?? DockerHub, key => new Lazy<IDockerRegistryAuthenticationConfiguration>(() => this.GetUncachedAuthConfig(key)));
      return lazyAuthConfig.Value;
    }

    private static string GetDefaultDockerConfigFilePath()
    {
      var dockerConfigDirectoryPath = PropertiesFileConfiguration.Instance.GetDockerConfig() ?? EnvironmentConfiguration.Instance.GetDockerConfig() ?? UserProfileDockerConfigDirectoryPath;
      return Path.Combine(dockerConfigDirectoryPath, "config.json");
    }

    [CanBeNull]
    private static JsonDocument GetDefaultDockerAuthConfig()
    {
      return PropertiesFileConfiguration.Instance.GetDockerAuthConfig() ?? EnvironmentConfiguration.Instance.GetDockerAuthConfig();
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
            authConfig = new IDockerRegistryAuthenticationProvider[]
              {
                new CredsHelperProvider(dockerConfigDocument, this.logger),
                new CredsStoreProvider(dockerConfigDocument, this.logger),
                new Base64Provider(dockerConfigDocument, this.logger),
                new Base64Provider(GetDefaultDockerAuthConfig() ?? JsonDocument.Parse("{}"), this.logger),
              }
              .AsParallel()
              .Select(authenticationProvider => authenticationProvider.GetAuthConfig(hostname))
              .FirstOrDefault(authenticationProvider => authenticationProvider != null);
          }
        }
      }
      else if (GetDefaultDockerAuthConfig() is JsonDocument defaultDockerConfigDocument)
      {
        using (defaultDockerConfigDocument)
        {
          authConfig = new Base64Provider(defaultDockerConfigDocument, this.logger).GetAuthConfig(hostname);
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
