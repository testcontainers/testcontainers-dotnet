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
      : base(new[] { DockerConfig, DockerHost, DockerAuthConfig, RyukDisabled, RyukContainerImage, HubImageNamePrefix }
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
