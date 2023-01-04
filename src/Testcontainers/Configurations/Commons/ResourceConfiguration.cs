namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IResourceConfiguration" />
  [PublicAPI]
  public class ResourceConfiguration : IResourceConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="labels">The test session id.</param>
    public ResourceConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      IReadOnlyDictionary<string, string> labels = null)
    {
      this.DockerEndpointAuthConfig = dockerEndpointAuthenticationConfiguration;
      this.Labels = labels;
      this.SessionId = labels != null && labels.TryGetValue(ResourceReaper.ResourceReaperSessionLabel, out var resourceReaperSessionId) && Guid.TryParseExact(resourceReaperSessionId, "D", out var sessionId) ? sessionId : Guid.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    protected ResourceConfiguration(IResourceConfiguration resourceConfiguration)
      : this(new ResourceConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    protected ResourceConfiguration(IResourceConfiguration oldValue, IResourceConfiguration newValue)
      : this(
        dockerEndpointAuthenticationConfiguration: BuildConfiguration.Combine(oldValue.DockerEndpointAuthConfig, newValue.DockerEndpointAuthConfig),
        labels: BuildConfiguration.Combine(oldValue.Labels, newValue.Labels))
    {
    }

    /// <inheritdoc />
    public Guid SessionId { get; }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
