namespace Testcontainers.Bigtable;

/// <inheritdoc cref="ContainerConfiguration"/>
[PublicAPI]
public class BigtableEmulatorConfiguration: ContainerConfiguration
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  public BigtableEmulatorConfiguration()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public BigtableEmulatorConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public BigtableEmulatorConfiguration(IContainerConfiguration resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public BigtableEmulatorConfiguration(BigtableEmulatorConfiguration resourceConfiguration)
    : this(new BigtableEmulatorConfiguration(), resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="oldValue">The old Docker resource configuration.</param>
  /// <param name="newValue">The new Docker resource configuration.</param>
  public BigtableEmulatorConfiguration(BigtableEmulatorConfiguration oldValue, BigtableEmulatorConfiguration newValue)
    : base(oldValue, newValue)
  {
  }
}
