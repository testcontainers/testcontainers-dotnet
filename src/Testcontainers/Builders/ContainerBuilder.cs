namespace DotNet.Testcontainers.Builders
{
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using System;

  /// <summary>
  /// A fluent Docker container builder.
  /// </summary>
  /// <remarks>
  /// The container builder configuration requires image tag to be provided. Either invoke the constructor with image tag as a parameter or call the <see cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}.WithImage(string)" /> method after.
  /// </remarks>
  /// <example>
  ///   The default configuration is equivalent to:
  ///   <code>
  ///   _ = new ContainerBuilder()
  ///     .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
  ///     .WithLabel(DefaultLabels.Instance)
  ///     .WithCleanUp(true)
  ///     .WithImagePullPolicy(PullPolicy.Missing)
  ///     .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
  ///     .WithWaitStrategy(Wait.ForUnixContainer())
  ///     .WithStartupCallback((_, ct) => Task.CompletedTask)
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public class ContainerBuilder : ContainerBuilder<ContainerBuilder, IContainer, IContainerConfiguration>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    [Obsolete("Prefer constructor with image as a parameter instead.")]
    public ContainerBuilder()
      : this(new ContainerConfiguration())
    {
      DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag.</param>
    public ContainerBuilder(string image)
      : this(new ContainerConfiguration())
    {
      DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public ContainerBuilder(IImage image)
      : this(new ContainerConfiguration())
    {
      DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    protected ContainerBuilder(IContainerConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override IContainerConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override IContainer Build()
    {
      Validate();
      return new DockerContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected sealed override ContainerBuilder Init()
    {
      return base.Init();
    }

    /// <inheritdoc />
    protected override ContainerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ContainerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ContainerBuilder Merge(IContainerConfiguration oldValue, IContainerConfiguration newValue)
    {
      return new ContainerBuilder(new ContainerConfiguration(oldValue, newValue));
    }
  }
}
