namespace Testcontainers.Bigtable;

/// <inheritdoc cref="ContainerConfiguration"/>
[PublicAPI]
public class BigtableConfiguration: ContainerConfiguration
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  public BigtableConfiguration()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public BigtableConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public BigtableConfiguration(IContainerConfiguration resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public BigtableConfiguration(BigtableConfiguration resourceConfiguration)
    : this(new BigtableConfiguration(), resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="oldValue">The old Docker resource configuration.</param>
  /// <param name="newValue">The new Docker resource configuration.</param>
  public BigtableConfiguration(BigtableConfiguration oldValue, BigtableConfiguration newValue)
    : base(oldValue, newValue)
  {
  }
}
