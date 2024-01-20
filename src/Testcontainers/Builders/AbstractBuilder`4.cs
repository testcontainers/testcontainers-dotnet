namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// An abstract fluent Docker resource builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  /// <typeparam name="TResourceEntity">The resource entity.</typeparam>
  /// <typeparam name="TCreateResourceEntity">The underlying Docker.DotNet resource entity.</typeparam>
  /// <typeparam name="TConfigurationEntity">The configuration entity.</typeparam>
  [PublicAPI]
  public abstract class AbstractBuilder<TBuilderEntity, TResourceEntity, TCreateResourceEntity, TConfigurationEntity> : IAbstractBuilder<TBuilderEntity, TResourceEntity, TCreateResourceEntity>
    where TBuilderEntity : IAbstractBuilder<TBuilderEntity, TResourceEntity, TCreateResourceEntity>
    where TConfigurationEntity : IResourceConfiguration<TCreateResourceEntity>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractBuilder{TBuilderEntity, TResourceEntity, TCreateResourceEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    protected AbstractBuilder(TConfigurationEntity dockerResourceConfiguration)
    {
      _ = TestcontainersSettings.SettingsInitialized.WaitOne(TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// Gets the Docker resource configuration.
    /// </summary>
    protected abstract TConfigurationEntity DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public TBuilderEntity WithDockerEndpoint(string endpoint)
    {
      return WithDockerEndpoint(new Uri(endpoint));
    }

    /// <inheritdoc />
    public TBuilderEntity WithDockerEndpoint(Uri endpoint)
    {
      return WithDockerEndpoint(new DockerEndpointAuthenticationConfiguration(endpoint));
    }

    /// <inheritdoc />
    public TBuilderEntity WithDockerEndpoint(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      return Clone(new ResourceConfiguration<TCreateResourceEntity>(dockerEndpointAuthenticationConfiguration: dockerEndpointAuthConfig));
    }

    /// <inheritdoc />
    public TBuilderEntity WithCleanUp(bool cleanUp)
    {
      return WithResourceReaperSessionId(TestcontainersSettings.ResourceReaperEnabled && cleanUp ? ResourceReaper.DefaultSessionId : Guid.Empty);
    }

    /// <inheritdoc />
    public TBuilderEntity WithReuse(bool reuse)
    {
      return Clone(new ResourceConfiguration<TCreateResourceEntity>(reuse: reuse)).WithCleanUp(!reuse);
    }

    /// <inheritdoc />
    public TBuilderEntity WithLabel(string name, string value)
    {
      return WithLabel(new Dictionary<string, string> { { name, value } });
    }

    /// <inheritdoc />
    public TBuilderEntity WithLabel(IReadOnlyDictionary<string, string> labels)
    {
      return Clone(new ResourceConfiguration<TCreateResourceEntity>(labels: labels));
    }

    /// <inheritdoc />
    public TBuilderEntity WithCreateParameterModifier(Action<TCreateResourceEntity> parameterModifier)
    {
      var parameterModifiers = new[] { parameterModifier };
      return Clone(new ResourceConfiguration<TCreateResourceEntity>(parameterModifiers: parameterModifiers));
    }

    /// <inheritdoc />
    public abstract TResourceEntity Build();

    /// <summary>
    /// Sets the Resource Reaper session id.
    /// </summary>
    /// <param name="resourceReaperSessionId">The <see cref="ResourceReaper" /> session id.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected TBuilderEntity WithResourceReaperSessionId(Guid resourceReaperSessionId)
    {
      return WithLabel(ResourceReaper.ResourceReaperSessionLabel, resourceReaperSessionId.ToString("D"));
    }

    /// <summary>
    /// Initializes the fluent Docker resource builder.
    /// </summary>
    /// <remarks>
    /// Due to the immutable design decision the configuration cannot be kept during the object instantiation.
    /// To keep the configuration override <see cref="DockerResourceConfiguration" />.
    /// </remarks>
    /// <example>
    ///   <code>
    ///   public sealed class CustomBuilder : AbstractBuilder&lt;CustomBuilder, IContainer, IContainerConfiguration&gt;
    ///   {
    ///     public CustomBuilder() : base(new ContainerConfiguration())
    ///     {
    ///       this.DockerResourceConfiguration = this.Init().DockerResourceConfiguration;
    ///     }
    ///   <br />
    ///     protected override IContainerConfiguration DockerResourceConfiguration { get; }
    ///   <br />
    ///     protected override CustomBuilder Init()
    ///     {
    ///       return base.Init().WithLabel(&quot;custom.builder&quot;, bool.TrueString.ToLowerInvariant());
    ///     }
    ///   }
    ///   </code>
    /// </example>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected virtual TBuilderEntity Init()
    {
      return WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig).WithLabel(DefaultLabels.Instance);
    }

    /// <summary>
    /// Validates the Docker resource configuration.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when a mandatory Docker resource configuration is not set.</exception>
    protected virtual void Validate()
    {
      const string containerRuntimeNotFound = "Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured. You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file. For more information, visit:\nhttps://dotnet.testcontainers.org/custom_configuration/";
      _ = Guard.Argument(DockerResourceConfiguration.DockerEndpointAuthConfig, nameof(IResourceConfiguration<TCreateResourceEntity>.DockerEndpointAuthConfig))
        .ThrowIf(argument => argument.Value == null, argument => new ArgumentException(containerRuntimeNotFound, argument.Name));

      const string reuseNotSupported = "Reuse cannot be used in conjunction with WithCleanUp(true).";
      _ = Guard.Argument(DockerResourceConfiguration, nameof(IResourceConfiguration<TCreateResourceEntity>.Reuse))
        .ThrowIf(argument => argument.Value.Reuse.HasValue && argument.Value.Reuse.Value && !Guid.Empty.Equals(argument.Value.SessionId), argument => new ArgumentException(reuseNotSupported, argument.Name));
    }

    /// <summary>
    /// Clones the Docker resource builder configuration.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected abstract TBuilderEntity Clone(IResourceConfiguration<TCreateResourceEntity> resourceConfiguration);

    /// <summary>
    /// Merges the Docker resource builder configuration.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected abstract TBuilderEntity Merge(TConfigurationEntity oldValue, TConfigurationEntity newValue);
  }
}
