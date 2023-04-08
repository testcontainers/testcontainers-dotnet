namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IResourceConfiguration{TCreateResourceEntity}" />
  [PublicAPI]
  public class ResourceConfiguration<TCreateResourceEntity> : IResourceConfiguration<TCreateResourceEntity>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration{TCreateResourceEntity}" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="labels">The test session id.</param>
    /// <param name="parameterModifiers">A list of low level modifications of the Docker.DotNet entity.</param>
    public ResourceConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyList<Action<TCreateResourceEntity>> parameterModifiers = null)
    {
      SessionId = labels != null && labels.TryGetValue(ResourceReaper.ResourceReaperSessionLabel, out var resourceReaperSessionId) && Guid.TryParseExact(resourceReaperSessionId, "D", out var sessionId) ? sessionId : Guid.Empty;
      DockerEndpointAuthConfig = dockerEndpointAuthenticationConfiguration;
      Labels = labels;
      ParameterModifiers = parameterModifiers;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration{TCreateResourceEntity}" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    protected ResourceConfiguration(IResourceConfiguration<TCreateResourceEntity> resourceConfiguration)
      : this(new ResourceConfiguration<TCreateResourceEntity>(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration{TCreateResourceEntity}" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    protected ResourceConfiguration(IResourceConfiguration<TCreateResourceEntity> oldValue, IResourceConfiguration<TCreateResourceEntity> newValue)
      : this(
        dockerEndpointAuthenticationConfiguration: BuildConfiguration.Combine(oldValue.DockerEndpointAuthConfig, newValue.DockerEndpointAuthConfig),
        parameterModifiers: BuildConfiguration.Combine(oldValue.ParameterModifiers, newValue.ParameterModifiers),
        labels: BuildConfiguration.Combine(oldValue.Labels, newValue.Labels))
    {
    }

    /// <inheritdoc />
    public Guid SessionId { get; }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }

    /// <inheritdoc />
    public IReadOnlyList<Action<TCreateResourceEntity>> ParameterModifiers { get; }
  }
}
