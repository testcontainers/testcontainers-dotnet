namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IDockerResourceConfiguration" />
  internal class DockerResourceConfiguration : IDockerResourceConfiguration
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
      this.SessionId = labels != null && labels.TryGetValue(ResourceReaper.ResourceReaperSessionLabel, out var resourceReaperSessionId) && Guid.TryParseExact(resourceReaperSessionId, "D", out var sessionId) ? sessionId : Guid.Empty;
    }

    /// <inheritdoc />
    public Guid SessionId { get; }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
