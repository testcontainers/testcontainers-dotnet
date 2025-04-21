namespace Testcontainers.Tika;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class TikaConfiguration : ContainerConfiguration
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TikaConfiguration" /> class.
  /// </summary>
  /// <param name="timeout">The timeout for the Tika server.</param>
  public TikaConfiguration(int timeout = 30000)
  {
    Timeout = timeout;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TikaConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public TikaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
      : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TikaConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public TikaConfiguration(IContainerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TikaConfiguration" /> class,
  /// combining properties from two existing configurations.
  /// </summary>
  /// <param name="oldValue">The previous configuration values.</param>
  /// <param name="newValue">The new configuration values to merge with the old ones.</param>
  public TikaConfiguration(TikaConfiguration oldValue, TikaConfiguration newValue)
      : base(oldValue, newValue)
  {
    // Combine values manually
    Timeout = BuildConfiguration.Combine(oldValue.Timeout, newValue.Timeout);
  }


  /// <summary>
  /// Gets the Tika server timeout.
  /// </summary>
  public int Timeout { get; }
}
