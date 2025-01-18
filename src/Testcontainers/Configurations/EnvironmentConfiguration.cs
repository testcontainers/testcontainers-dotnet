namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// Reads and maps the custom configurations from the environment variables.
  /// </summary>
  internal class EnvironmentConfiguration : CustomConfiguration, ICustomConfiguration
  {
    private const string DockerConfig = "DOCKER_CONFIG";

    private const string DockerHost = "DOCKER_HOST";

    private const string DockerContext = "DOCKER_CONTEXT";

    private const string DockerAuthConfig = "DOCKER_AUTH_CONFIG";

    private const string DockerCertPath = "DOCKER_CERT_PATH";

    private const string DockerTls = "DOCKER_TLS";

    private const string DockerTlsVerify = "DOCKER_TLS_VERIFY";

    private const string DockerHostOverride = "TESTCONTAINERS_HOST_OVERRIDE";

    private const string DockerSocketOverride = "TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE";

    private const string RyukDisabled = "TESTCONTAINERS_RYUK_DISABLED";

    private const string RyukContainerPrivileged = "TESTCONTAINERS_RYUK_CONTAINER_PRIVILEGED";

    private const string RyukContainerImage = "TESTCONTAINERS_RYUK_CONTAINER_IMAGE";

    private const string HubImageNamePrefix = "TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX";

    private const string WaitStrategyRetries = "TESTCONTAINERS_WAIT_STRATEGY_RETRIES";

    private const string WaitStrategyInterval = "TESTCONTAINERS_WAIT_STRATEGY_INTERVAL";

    private const string WaitStrategyTimeout = "TESTCONTAINERS_WAIT_STRATEGY_TIMEOUT";

    static EnvironmentConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentConfiguration" /> class.
    /// </summary>
    public EnvironmentConfiguration()
      : base(new[]
        {
          DockerAuthConfig,
          DockerCertPath,
          DockerConfig,
          DockerHost,
          DockerContext,
          DockerTls,
          DockerTlsVerify,
          DockerHostOverride,
          DockerSocketOverride,
          RyukDisabled,
          RyukContainerPrivileged,
          RyukContainerImage,
          HubImageNamePrefix,
          WaitStrategyRetries,
          WaitStrategyInterval,
          WaitStrategyTimeout,
        }
        .ToDictionary(key => key, Environment.GetEnvironmentVariable))
    {
    }

    /// <summary>
    /// Gets the <see cref="ICustomConfiguration" /> instance.
    /// </summary>
    public static ICustomConfiguration Instance { get; }
      = new EnvironmentConfiguration();

    /// <inheritdoc />
    public string GetDockerConfig()
    {
      return GetDockerConfig(DockerConfig);
    }

    /// <inheritdoc />
    public Uri GetDockerHost()
    {
      return GetDockerHost(DockerHost);
    }

    /// <inheritdoc />
    public string GetDockerContext()
    {
      return GetDockerContext(DockerContext);
    }

    /// <inheritdoc />
    public string GetDockerHostOverride()
    {
      return GetDockerHostOverride(DockerHostOverride);
    }

    /// <inheritdoc />
    public string GetDockerSocketOverride()
    {
      return GetDockerSocketOverride(DockerSocketOverride);
    }

    /// <inheritdoc />
    public JsonDocument GetDockerAuthConfig()
    {
      return GetDockerAuthConfig(DockerAuthConfig);
    }

    /// <inheritdoc />
    public string GetDockerCertPath()
    {
      return GetDockerCertPath(DockerCertPath);
    }

    /// <inheritdoc />
    public bool GetDockerTls()
    {
      return GetDockerTls(DockerTls);
    }

    /// <inheritdoc />
    public bool GetDockerTlsVerify()
    {
      return GetDockerTlsVerify(DockerTlsVerify);
    }

    /// <inheritdoc />
    public bool GetRyukDisabled()
    {
      return GetRyukDisabled(RyukDisabled);
    }

    /// <inheritdoc />
    public bool? GetRyukContainerPrivileged()
    {
      return GetRyukContainerPrivileged(RyukContainerPrivileged);
    }

    /// <inheritdoc />
    public IImage GetRyukContainerImage()
    {
      return GetRyukContainerImage(RyukContainerImage);
    }

    /// <inheritdoc />
    public string GetHubImageNamePrefix()
    {
      return GetHubImageNamePrefix(HubImageNamePrefix);
    }

    /// <inheritdoc />
    public ushort? GetWaitStrategyRetries()
    {
      return GetWaitStrategyRetries(WaitStrategyRetries);
    }

    /// <inheritdoc />
    public TimeSpan? GetWaitStrategyInterval()
    {
      return GetWaitStrategyInterval(WaitStrategyInterval);
    }

    /// <inheritdoc />
    public TimeSpan? GetWaitStrategyTimeout()
    {
      return GetWaitStrategyTimeout(WaitStrategyTimeout);
    }
  }
}
