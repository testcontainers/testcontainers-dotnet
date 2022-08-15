namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [PublicAPI]
  public class AzuriteTestcontainerConfiguration
  {
    // TODO: Add support for these options based on request
    // azurite.cert Path to a PEM or PFX cert file.Required by HTTPS mode.
    // azurite.key Path to a PEM key file. Required when azurite.cert points to a PEM file.
    // azurite.pwd PFX cert password. Required when azurite.cert points to a PFX file.
    // azurite.oauth OAuth authentication level. Candidate level values: basic.
    // AZURITE_ACCOUNTS environment variable to set account names and keys.

    /// <summary>
    /// Default Workspace location folder path. Default is /data.
    /// </summary>
    [PublicAPI]
    public const string DefaultLocation = "/data";

    internal const string DefaultBlobEndpoint = "0.0.0.0";
    internal const string DefaultQueueEndpoint = "0.0.0.0";
    internal const string DefaultTableEndpoint = "0.0.0.0";

    private const int DefaultBlobPort = 10000;
    private const int DefaultQueuePort = 10001;
    private const int DefaultTablePort = 10002;
    private const string DefaultAzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.18.0";

    private bool blobServiceOnlyEnabled;
    private bool queueServiceOnlyEnabled;
    private bool tableServiceOnlyEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainerConfiguration" /> class with default Azurite image.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public AzuriteTestcontainerConfiguration()
      : this(DefaultAzuriteImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public AzuriteTestcontainerConfiguration(string image)
    {
      this.Image = image;
    }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [PublicAPI]
    public string Image { get; }

    /// <summary>
    /// Gets or sets the host Blob port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the container blob port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int BlobPort { get; set; }

    /// <summary>
    /// Gets or sets the container Blob port.
    /// </summary>
    [PublicAPI]
    public int BlobContainerPort { get; set; } = DefaultBlobPort;

    /// <summary>
    /// Gets or sets a value indicating whether Blob service should run standalone.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool BlobServiceOnlyEnabled
    {
      get => this.blobServiceOnlyEnabled;
      set
      {
        this.blobServiceOnlyEnabled = value;
        if (value)
        {
          this.QueueServiceOnlyEnabled = this.TableServiceOnlyEnabled = false;
        }
      }
    }

    /// <summary>
    /// Gets or sets the host Queue port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int QueuePort { get; set; }

    /// <summary>
    /// Gets or sets the container Queue port.
    /// </summary>
    [PublicAPI]
    public int QueueContainerPort { get; set; } = DefaultQueuePort;

    /// <summary>
    /// Gets or sets a value indicating whether Queue service should run standalone.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool QueueServiceOnlyEnabled
    {
      get => this.queueServiceOnlyEnabled;
      set
      {
        this.queueServiceOnlyEnabled = value;
        if (value)
        {
          this.BlobServiceOnlyEnabled = this.TableServiceOnlyEnabled = false;
        }
      }
    }

    /// <summary>
    /// Gets or sets the host Table port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int TablePort { get; set; }

    /// <summary>
    /// Gets or sets the container Table port.
    /// </summary>
    [PublicAPI]
    public int TableContainerPort { get; set; } = DefaultTablePort;

    /// <summary>
    /// Gets or sets a value indicating whether Table service should run standalone.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool TableServiceOnlyEnabled
    {
      get => this.tableServiceOnlyEnabled;
      set
      {
        this.tableServiceOnlyEnabled = value;
        if (value)
        {
          this.BlobServiceOnlyEnabled = this.QueueServiceOnlyEnabled = false;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether all Azurite service will run.
    /// </summary>
    [PublicAPI]
    public bool AllServicesEnabled => !this.BlobServiceOnlyEnabled && !this.QueueServiceOnlyEnabled && !this.TableServiceOnlyEnabled;

    /// <summary>
    /// Gets or sets workspace location path.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default workspace location of the hosted service.
    /// </remarks>
    [PublicAPI]
    [CanBeNull]
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether disable access log in console. By default false.
    /// </summary>
    [PublicAPI]
    public bool SilentModeEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether loose mode should be enabled.
    /// Enable loose mode which ignores unsupported headers and parameters.
    /// By default false.
    /// </summary>
    [PublicAPI]
    public bool LooseModeEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether skip the request API version check. By default false.
    /// </summary>
    [PublicAPI]
    public bool SkipApiVersionCheckEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether force parsing storage account name from request Uri path, instead of from request Uri host. By default false.
    /// </summary>
    [PublicAPI]
    public bool ProductStyleUrlDisabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether debug log should be enabled.
    /// Logs are stored in debug.log file in workspace location, by default /data/debug.log.
    /// By default false.
    /// </summary>
    [PublicAPI]
    public bool DebugEnabled { get; set; }

    /// <summary>
    /// Gets the wait strategy.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Wait.ForUnixContainer" /> and waits for Azurite ports.
    /// </remarks>
    [PublicAPI]
    public IWaitForContainerOS WaitStrategy
    {
      get
      {
        var waitStrategy = Wait.ForUnixContainer();
        waitStrategy = this.BlobServiceOnlyEnabled || this.AllServicesEnabled ? waitStrategy.UntilPortIsAvailable(this.BlobContainerPort) : waitStrategy;
        waitStrategy = this.QueueServiceOnlyEnabled || this.AllServicesEnabled ? waitStrategy.UntilPortIsAvailable(this.QueueContainerPort) : waitStrategy;
        waitStrategy = this.TableServiceOnlyEnabled || this.AllServicesEnabled ? waitStrategy.UntilPortIsAvailable(this.TableContainerPort) : waitStrategy;
        return waitStrategy;
      }
    }
  }
}
