namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AzuriteConfiguration : ContainerConfiguration
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="azuriteServices">A value indicating which Azurite service is turned on.</param>
  /// <param name="blobPort">The Azurite container blob port.</param>
  /// <param name="blobPortBinding">The host blob port to bind.</param>
  /// <param name="queuePort">The Azurite container queue port.</param>
  /// <param name="queuePortBinding">The host queue port to bind.</param>
  /// <param name="tablePort">The Azurite container table port.</param>
  /// <param name="tablePortBinding">The host table port to bind.</param>
  /// <param name="workspaceLocation">The Azurite workspace directory path.</param>
  /// <param name="workspaceLocationBinding">The directory path where to bind the Azurite workspace directory.</param>
  /// <param name="debugModeEnabled">A value indicating whether the Azurite debug mode is enabled or not</param>
  /// <param name="silentModeEnabled">A value indicating whether the Azurite silent mode is enabled or not.</param>
  /// <param name="looseModeEnabled">A value indicating whether the Azurite loose mode is enabled or not.</param>
  /// <param name="productStyleUrlDisabled">A value indicating whether the Azurite skip API version check is enabled or not.</param>
  /// <param name="skipApiVersionCheckEnabled">A value indicating whether the Azurite product style URL is enabled or not.</param>
  public AzuriteConfiguration(
    AzuriteServices? azuriteServices = null,
    int? blobPort = null,
    int? blobPortBinding = null,
    int? queuePort = null,
    int? queuePortBinding = null,
    int? tablePort = null,
    int? tablePortBinding = null,
    string workspaceLocation = null,
    string workspaceLocationBinding = null,
    bool? debugModeEnabled = null,
    bool? silentModeEnabled = null,
    bool? looseModeEnabled = null,
    bool? productStyleUrlDisabled = null,
    bool? skipApiVersionCheckEnabled = null)
  {
    AzuriteServices = azuriteServices;
    BlobPort = blobPort;
    BlobPortBinding = blobPortBinding;
    QueuePort = queuePort;
    QueuePortBinding = queuePortBinding;
    TablePort = tablePort;
    TablePortBinding = tablePortBinding;
    WorkspaceLocation = workspaceLocation;
    WorkspaceLocationBinding = workspaceLocationBinding;
    DebugModeEnabled = debugModeEnabled;
    SilentModeEnabled = silentModeEnabled;
    LooseModeEnabled = looseModeEnabled;
    ProductStyleUrlDisabled = productStyleUrlDisabled;
    SkipApiVersionCheckEnabled = skipApiVersionCheckEnabled;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public AzuriteConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public AzuriteConfiguration(IContainerConfiguration resourceConfiguration)
    : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public AzuriteConfiguration(AzuriteConfiguration resourceConfiguration)
    : this(new AzuriteConfiguration(), resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
  /// </summary>
  /// <param name="oldValue">The old Docker resource configuration.</param>
  /// <param name="newValue">The new Docker resource configuration.</param>
  public AzuriteConfiguration(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
    : base(oldValue, newValue)
  {
    AzuriteServices = BuildConfiguration.Combine(oldValue.AzuriteServices, newValue.AzuriteServices);
    BlobPort = BuildConfiguration.Combine(oldValue.BlobPort, newValue.BlobPort);
    BlobPortBinding = BuildConfiguration.Combine(oldValue.BlobPortBinding, newValue.BlobPortBinding);
    QueuePort = BuildConfiguration.Combine(oldValue.QueuePort, newValue.QueuePort);
    QueuePortBinding = BuildConfiguration.Combine(oldValue.QueuePortBinding, newValue.QueuePortBinding);
    TablePort = BuildConfiguration.Combine(oldValue.TablePort, newValue.TablePort);
    TablePortBinding = BuildConfiguration.Combine(oldValue.TablePortBinding, newValue.TablePortBinding);
    WorkspaceLocation = BuildConfiguration.Combine(oldValue.WorkspaceLocation, newValue.WorkspaceLocation);
    WorkspaceLocationBinding = BuildConfiguration.Combine(oldValue.WorkspaceLocationBinding, newValue.WorkspaceLocationBinding);
    DebugModeEnabled = BuildConfiguration.Combine(oldValue.DebugModeEnabled, newValue.DebugModeEnabled);
    SilentModeEnabled = BuildConfiguration.Combine(oldValue.SilentModeEnabled, newValue.SilentModeEnabled);
    LooseModeEnabled = BuildConfiguration.Combine(oldValue.LooseModeEnabled, newValue.LooseModeEnabled);
    ProductStyleUrlDisabled = BuildConfiguration.Combine(oldValue.ProductStyleUrlDisabled, newValue.ProductStyleUrlDisabled);
    SkipApiVersionCheckEnabled = BuildConfiguration.Combine(oldValue.SkipApiVersionCheckEnabled, newValue.SkipApiVersionCheckEnabled);
  }

  /// <summary>
  ///   Gets a value indicating which Azurite service is turned on.
  /// </summary>
  /// <remarks>
  ///   Default value is <see cref="AzuriteServices" />.All.
  /// </remarks>
  public AzuriteServices? AzuriteServices { get; }

  /// <summary>
  ///   Gets the Azurite container blob port.
  /// </summary>
  public int? BlobPort { get; }

  /// <summary>
  ///   Gets the host blob port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container blob port.
  /// </remarks>
  public int? BlobPortBinding { get; }

  /// <summary>
  ///   Gets the Azurite container queue port.
  /// </summary>
  public int? QueuePort { get; }

  /// <summary>
  ///   Gets the host queue port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container queue port.
  /// </remarks>
  public int? QueuePortBinding { get; }

  /// <summary>
  ///   Gets the Azurite container table port.
  /// </summary>
  public int? TablePort { get; }

  /// <summary>
  ///   Gets the host table port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container table port.
  /// </remarks>
  public int? TablePortBinding { get; }

  /// <summary>
  ///   Gets the Azurite workspace directory path.
  /// </summary>
  /// <remarks>
  ///   Corresponds to the default workspace directory path.
  /// </remarks>
  [CanBeNull]
  public string WorkspaceLocation { get; }

  /// <summary>
  ///   Gets the directory path where to bind the Azurite workspace directory.
  /// </summary>
  [CanBeNull]
  public string WorkspaceLocationBinding { get; }

  /// <summary>
  ///   Gets a value indicating whether debug mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Writes logs to the workspace directory path.
  ///   Default value is false.
  /// </remarks>
  public bool? DebugModeEnabled { get; }

  /// <summary>
  ///   Gets a value indicating whether silent mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public bool? SilentModeEnabled { get; }

  /// <summary>
  ///   Gets a value indicating whether loose mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public bool? LooseModeEnabled { get; }

  /// <summary>
  ///   Gets a value indicating whether skip API version check is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public bool? SkipApiVersionCheckEnabled { get; }

  /// <summary>
  ///   Gets a value indicating whether product style URL is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Parses storage account name from the URI path, instead of the URI host.
  ///   Default value is false.
  /// </remarks>
  public bool? ProductStyleUrlDisabled { get; }
}