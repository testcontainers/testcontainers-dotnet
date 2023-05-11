namespace Testcontainers.Spanner;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public class SpannerConfiguration : ContainerConfiguration
{
  public string? ProjectId { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerConfiguration" /> class.
  /// </summary>
  /// <param name="projectId"></param>
  public SpannerConfiguration(string? projectId = null)
  {
    ProjectId = projectId;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public SpannerConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
      : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public SpannerConfiguration(IContainerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public SpannerConfiguration(SpannerConfiguration resourceConfiguration)
      : this(new SpannerConfiguration(), resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerConfiguration" /> class.
  /// </summary>
  /// <param name="oldValue">The old Docker resource configuration.</param>
  /// <param name="newValue">The new Docker resource configuration.</param>
  public SpannerConfiguration(SpannerConfiguration oldValue, SpannerConfiguration newValue)
      : base(oldValue, newValue)
  {
    ProjectId = BuildConfiguration.Combine(oldValue.ProjectId, newValue.ProjectId);
  }
}
