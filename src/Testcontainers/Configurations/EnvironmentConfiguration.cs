namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// Reads and maps the custom configurations from the environment variables.
  /// </summary>
  internal sealed class EnvironmentConfiguration : CustomConfiguration, ICustomConfiguration
  {
    private const string DockerConfig = "DOCKER_CONFIG";

    private const string DockerHost = "DOCKER_HOST";

    private const string DockerAuthConfig = "DOCKER_AUTH_CONFIG";

    private const string DockerCertPath = "DOCKER_CERT_PATH";

    private const string DockerTls = "DOCKER_TLS";

    private const string DockerTlsVerify = "DOCKER_TLS_VERIFY";

    private const string RyukDisabled = "TESTCONTAINERS_RYUK_DISABLED";

    private const string RyukContainerImage = "TESTCONTAINERS_RYUK_CONTAINER_IMAGE";

    private const string HubImageNamePrefix = "TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX";

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
          DockerTls,
          DockerTlsVerify,
          RyukDisabled,
          RyukContainerImage,
          HubImageNamePrefix,
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
      return this.GetDockerConfig(DockerConfig);
    }

    /// <inheritdoc />
    public Uri GetDockerHost()
    {
      return this.GetDockerHost(DockerHost);
    }

    /// <inheritdoc />
    public JsonDocument GetDockerAuthConfig()
    {
      return this.GetDockerAuthConfig(DockerAuthConfig);
    }

    /// <inheritdoc />
    public string GetDockerCertPath()
    {
      return this.GetDockerCertPath(DockerCertPath);
    }

    /// <inheritdoc />
    public bool GetDockerTls()
    {
      return this.GetDockerTls(DockerTls);
    }

    /// <inheritdoc />
    public bool GetDockerTlsVerify()
    {
      return this.GetDockerTlsVerify(DockerTlsVerify);
    }

    /// <inheritdoc />
    public bool GetRyukDisabled()
    {
      return this.GetRyukDisabled(RyukDisabled);
    }

    /// <inheritdoc />
    public IDockerImage GetRyukContainerImage()
    {
      return this.GetRyukContainerImage(RyukContainerImage);
    }

    /// <inheritdoc />
    public string GetHubImageNamePrefix()
    {
      return this.GetHubImageNamePrefix(HubImageNamePrefix);
    }
  }
}
