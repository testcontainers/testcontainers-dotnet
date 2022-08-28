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

    private readonly FileInfo dockerConfigFilePath;

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
    /// <param name="dockerConfigFilePath">The Docker config file path.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(string dockerConfigFilePath, ILogger logger)
      : this(new FileInfo(dockerConfigFilePath), logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="dockerConfigFilePath">The Docker config file path.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(FileInfo dockerConfigFilePath, ILogger logger)
    {
      this.dockerConfigFilePath = dockerConfigFilePath;
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

    private static JsonDocument GetDefaultDockerAuthConfig()
    {
      return PropertiesFileConfiguration.Instance.GetDockerAuthConfig() ?? EnvironmentConfiguration.Instance.GetDockerAuthConfig() ?? JsonDocument.Parse("{}");
    }

    private IDockerRegistryAuthenticationConfiguration GetUncachedAuthConfig(string hostname)
    {
      using (var dockerAuthConfigJsonDocument = GetDefaultDockerAuthConfig())
      {
        IDockerRegistryAuthenticationConfiguration authConfig;

        if (this.dockerConfigFilePath.Exists)
        {
          using (var dockerConfigFileStream = new FileStream(this.dockerConfigFilePath.FullName, FileMode.Open, FileAccess.Read))
          {
            using (var dockerConfigJsonDocument = JsonDocument.Parse(dockerConfigFileStream))
            {
              authConfig = new IDockerRegistryAuthenticationProvider[]
                {
                  new CredsHelperProvider(dockerConfigJsonDocument, this.logger),
                  new CredsStoreProvider(dockerConfigJsonDocument, this.logger),
                  new Base64Provider(dockerConfigJsonDocument, this.logger),
                  new Base64Provider(dockerAuthConfigJsonDocument, this.logger),
                }
                .AsParallel()
                .Select(authenticationProvider => authenticationProvider.GetAuthConfig(hostname))
                .FirstOrDefault(authenticationProvider => authenticationProvider != null);
            }
          }
        }
        else
        {
          this.logger.DockerConfigFileNotFound(this.dockerConfigFilePath.FullName);
          IDockerRegistryAuthenticationProvider authConfigProvider = new Base64Provider(dockerAuthConfigJsonDocument, this.logger);
          authConfig = authConfigProvider.GetAuthConfig(hostname);
        }

        if (authConfig != null)
        {
          return authConfig;
        }

        this.logger.DockerRegistryCredentialNotFound(hostname);
        return default(DockerRegistryAuthenticationConfiguration);
      }
    }
  }
}
