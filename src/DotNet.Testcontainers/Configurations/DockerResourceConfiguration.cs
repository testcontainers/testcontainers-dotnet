namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;

  /// <inheritdoc cref="IDockerResourceConfiguration" />
  public class DockerResourceConfiguration : IDockerResourceConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerResourceConfiguration" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    public DockerResourceConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
      : this(dockerResourceConfiguration.DockerEndpointAuthConfig, dockerResourceConfiguration.Labels)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerResourceConfiguration" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="labels">A list of labels.</param>
    public DockerResourceConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      IReadOnlyDictionary<string, string> labels = null)
    {
      this.DockerEndpointAuthConfig = dockerEndpointAuthenticationConfiguration;
      this.Labels = labels;
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
