namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  /// <summary>
  /// An abstract fluent Docker resource builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  /// <typeparam name="TConfigurationEntity">The configuration entity.</typeparam>
  public abstract class AbstractBuilder<TBuilderEntity, TConfigurationEntity>
    where TBuilderEntity : IAbstractBuilder<TBuilderEntity>
    where TConfigurationEntity : IDockerResourceConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractBuilder{TBuilderEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    protected AbstractBuilder(TConfigurationEntity dockerResourceConfiguration)
    {
      this.DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <summary>
    /// Gets the Docker resource configuration.
    /// </summary>
    protected TConfigurationEntity DockerResourceConfiguration { get; }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity}" />
    public TBuilderEntity WithDockerEndpoint(string endpoint)
    {
      return this.WithDockerEndpoint(new Uri(endpoint));
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity}" />
    public TBuilderEntity WithDockerEndpoint(Uri endpoint)
    {
      return this.WithDockerEndpoint(new DockerEndpointAuthenticationConfiguration(endpoint));
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity}" />
    public TBuilderEntity WithDockerEndpoint(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      return this.MergeNewConfiguration(new DockerResourceConfiguration(dockerEndpointAuthenticationConfiguration: dockerEndpointAuthConfig));
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity}" />
    public TBuilderEntity WithCleanUp(bool cleanUp)
    {
      return this.WithResourceReaperSessionId(TestcontainersSettings.ResourceReaperEnabled && cleanUp ? ResourceReaper.DefaultSessionId : Guid.Empty);
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity}" />
    public TBuilderEntity WithLabel(string name, string value)
    {
      var labels = new Dictionary<string, string> { { name, value } };
      return this.MergeNewConfiguration(new DockerResourceConfiguration(labels: labels));
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity}" />
    public TBuilderEntity WithResourceReaperSessionId(Guid resourceReaperSessionId)
    {
      return this.WithLabel(ResourceReaper.ResourceReaperSessionLabel, resourceReaperSessionId.ToString("D"));
    }

    /// <summary>
    /// Merges the current with the new Docker resource configuration.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The new Docker resource configuration.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    protected abstract TBuilderEntity MergeNewConfiguration(IDockerResourceConfiguration dockerResourceConfiguration);
  }
}
