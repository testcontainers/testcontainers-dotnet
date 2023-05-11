namespace Testcontainers.Spanner;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SpannerBuilder : ContainerBuilder<SpannerBuilder, SpannerContainer, SpannerConfiguration>
{
  private const string DefaultProjectId = "my-project";
  private const string SpannerEmulatorImage = "gcr.io/cloud-spanner-emulator/emulator:1.5.3";

  
  internal const int InternalGrpcPort = 9010;
  internal const int InternalRestPort = 9020;


  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
  /// </summary>
  public SpannerBuilder()
        : this(new SpannerConfiguration())
  {

    DockerResourceConfiguration = Init().DockerResourceConfiguration;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  private SpannerBuilder(SpannerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
  {
    DockerResourceConfiguration = resourceConfiguration;
  }

  // /// <inheritdoc />
  protected override SpannerConfiguration DockerResourceConfiguration { get; }

  /// <summary>
  /// Sets the ProjectId.
  /// </summary>
  /// <param name="projectId">The ProjectId.</param>
  /// <returns>A configured instance of <see cref="SpannerBuilder" />.</returns>
  public SpannerBuilder WithProjectId(string projectId)
    => Merge(DockerResourceConfiguration, new SpannerConfiguration(projectId: projectId));


  /// <inheritdoc />
  public override SpannerContainer Build()
  {
    Validate();
    return new SpannerContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
  }


  /// <inheritdoc />
  protected override SpannerBuilder Init()
  {
    return base.Init()
      .WithImage(SpannerEmulatorImage)
      .WithPortBinding(InternalGrpcPort, true)
      .WithPortBinding(InternalRestPort, true)
      .WithProjectId(DefaultProjectId)
      .WithWaitStrategy(
        Wait
          .ForUnixContainer()
          // The default wait for port implementation keeps waiting untill the test times out, therefor now using this custom flavor of the same concept
          .UntilMessageIsLogged($".+REST server listening at 0.0.0.0:{InternalRestPort}")
          .UntilMessageIsLogged($".+gRPC server listening at 0.0.0.0:{InternalGrpcPort}")
        );

  }


  /// <inheritdoc />
  protected override void Validate()
  {
    base.Validate();

    _ = Guard.Argument(DockerResourceConfiguration.ProjectId, nameof(DockerResourceConfiguration.ProjectId))
      .NotNull()
      .NotEmpty();
  }

  /// <inheritdoc />
  protected override SpannerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new SpannerConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override SpannerBuilder Clone(IContainerConfiguration resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new SpannerConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override SpannerBuilder Merge(SpannerConfiguration oldValue, SpannerConfiguration newValue)
  {
    return new SpannerBuilder(new SpannerConfiguration(oldValue, newValue));
  }
}
