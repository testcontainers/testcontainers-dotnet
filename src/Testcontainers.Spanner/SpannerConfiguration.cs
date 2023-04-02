namespace Testcontainers.Spanner;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class SpannerConfiguration : ContainerConfiguration
{
  public string? ProjectId { get; }
  public string? InstanceId { get; }
  public string? DatabaseId { get; }


  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerConfiguration" /> class.
  /// </summary>
  /// <param name="projectId"></param>
  /// <param name="instanceId"></param>
  /// <param name="databaseId"></param>
  public SpannerConfiguration(string? projectId = null, string? instanceId = null, string? databaseId = null)
  {
    ProjectId = projectId;
    InstanceId = instanceId;
    DatabaseId = databaseId;
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
    InstanceId = BuildConfiguration.Combine(oldValue.InstanceId, newValue.InstanceId);
    DatabaseId = BuildConfiguration.Combine(oldValue.DatabaseId, newValue.DatabaseId);
  }
}
