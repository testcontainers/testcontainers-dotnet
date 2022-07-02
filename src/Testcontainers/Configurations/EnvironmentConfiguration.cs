namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// Reads and maps the custom configurations from the environment variables.
  /// </summary>
  internal sealed class EnvironmentConfiguration : CustomConfiguration, ICustomConfiguration
  {
    private const string DockerConfig = "DOCKER_CONFIG";

    private const string DockerHost = "DOCKER_HOST";

    private const string RyukDisabled = "TESTCONTAINERS_RYUK_DISABLED";

    private const string RyukContainerImage = "TESTCONTAINERS_RYUK_CONTAINER_IMAGE";

    private const string HubImageNamePrefix = "TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX";

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentConfiguration" /> class.
    /// </summary>
    public EnvironmentConfiguration()
      : base(new[] { DockerConfig, DockerHost, RyukDisabled, RyukContainerImage, HubImageNamePrefix }
        .ToDictionary(key => key, Environment.GetEnvironmentVariable))
    {
    }

    /// <inheritdoc />
    public Uri GetDockerHost()
    {
      return this.GetDockerHost(DockerHost);
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
