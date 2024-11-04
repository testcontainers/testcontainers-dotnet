namespace Testcontainers.Playwright;

/// <inheritdoc cref="PlaywrightConfiguration" />
[PublicAPI]
public class PlaywrightConfiguration : ContainerConfiguration
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightConfiguration" /> class.
  /// </summary>
  /// <param name="endpoint">The Playwright endpoint.</param>
  public PlaywrightConfiguration(string endpoint = null,
    int? port = null)
  {
    Endpoint = endpoint;
    Port = port;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public PlaywrightConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public PlaywrightConfiguration(IContainerConfiguration resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public PlaywrightConfiguration(PlaywrightConfiguration resourceConfiguration)
    : this(new PlaywrightConfiguration(), resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightConfiguration" /> class.
  /// </summary>
  /// <param name="oldValue">The old Docker resource configuration.</param>
  /// <param name="newValue">The new Docker resource configuration.</param>
  public PlaywrightConfiguration(PlaywrightConfiguration oldValue, PlaywrightConfiguration newValue)
    : base(oldValue, newValue)
  {
    Endpoint = BuildConfiguration.Combine(oldValue.Endpoint, newValue.Endpoint);
  }


  /// <summary>
  /// Gets the Playwright endpoint.
  /// </summary>
  public string Endpoint { get; }


  /// <summary>
  /// Gets the Playwright port.
  /// </summary>
  public int? Port { get; }
}
