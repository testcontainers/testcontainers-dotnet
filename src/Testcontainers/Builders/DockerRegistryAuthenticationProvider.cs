namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Concurrent;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class DockerRegistryAuthenticationProvider : IDockerRegistryAuthenticationProvider
  {
    private const string DockerHub = "https://index.docker.io/v1/";

    private static readonly ConcurrentDictionary<string, Lazy<IDockerRegistryAuthenticationConfiguration>> Credentials = new ConcurrentDictionary<string, Lazy<IDockerRegistryAuthenticationConfiguration>>();

    private readonly DockerConfig _dockerConfig;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(ILogger logger)
      : this(DockerConfig.Instance, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="dockerConfig">The Docker config.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public DockerRegistryAuthenticationProvider(DockerConfig dockerConfig, ILogger logger)
    {
      _dockerConfig = dockerConfig;
      _logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return true;
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      var lazyAuthConfig = Credentials.GetOrAdd(hostname ?? DockerHub, key => new Lazy<IDockerRegistryAuthenticationConfiguration>(() => GetUncachedAuthConfig(key)));
      return lazyAuthConfig.Value;
    }

    private static JsonDocument GetDefaultDockerAuthConfig()
    {
      return EnvironmentConfiguration.Instance.GetDockerAuthConfig() ?? PropertiesFileConfiguration.Instance.GetDockerAuthConfig() ?? JsonDocument.Parse("{}");
    }

    private IDockerRegistryAuthenticationConfiguration GetUncachedAuthConfig(string hostname)
    {
      using (var dockerAuthConfigJsonDocument = GetDefaultDockerAuthConfig())
      {
        IDockerRegistryAuthenticationConfiguration authConfig;

        if (_dockerConfig.Exists)
        {
          using (var dockerConfigJsonDocument = _dockerConfig.Parse())
          {
            authConfig = new IDockerRegistryAuthenticationProvider[]
              {
                new CredsHelperProvider(dockerConfigJsonDocument, _logger),
                new CredsStoreProvider(dockerConfigJsonDocument, _logger),
                new Base64Provider(dockerConfigJsonDocument, _logger),
                new Base64Provider(dockerAuthConfigJsonDocument, _logger),
              }
              .AsParallel()
              .Select(authenticationProvider => authenticationProvider.GetAuthConfig(hostname))
              .FirstOrDefault(authenticationProvider => authenticationProvider != null);
          }
        }
        else
        {
          _logger.DockerConfigFileNotFound(_dockerConfig.FullName);
          IDockerRegistryAuthenticationProvider authConfigProvider = new Base64Provider(dockerAuthConfigJsonDocument, _logger);
          authConfig = authConfigProvider.GetAuthConfig(hostname);
        }

        if (authConfig != null)
        {
          return authConfig;
        }

        _logger.DockerRegistryCredentialNotFound(hostname);
        return default(DockerRegistryAuthenticationConfiguration);
      }
    }
  }
}
