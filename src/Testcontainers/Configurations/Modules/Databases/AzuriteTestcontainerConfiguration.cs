namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [PublicAPI]
  public class AzuriteTestcontainerConfiguration
  {
    /// <summary>
    /// Default Workspace location folder path. Default is /data.
    /// </summary>
    [PublicAPI]
    public const string DefaultLocation = "/data";

    private const int DefaultBlobPort = 10000;
    private const int DefaultQueuePort = 10001;
    private const int DefaultTablePort = 10002;
    private const string DefaultAzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.18.0";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainerConfiguration" /> class with default Azurite image.
    /// </summary>
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

    [Flags]
    internal enum AzuriteServices
    {
      None = 0,
      Blob = 1,
      Queue = 2,
      Table = 4,
      All = 7,
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
      get { return AzuriteServices.Blob.Equals(this.EnabledServices); }
      set { this.EnabledServices = value ? AzuriteServices.Blob : AzuriteServices.All; }
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
      get { return AzuriteServices.Queue.Equals(this.EnabledServices); }
      set { this.EnabledServices = value ? AzuriteServices.Queue : AzuriteServices.All; }
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
      get { return AzuriteServices.Table.Equals(this.EnabledServices); }
      set { this.EnabledServices = value ? AzuriteServices.Table : AzuriteServices.All; }
    }

    /// <summary>
    /// Gets a value indicating whether all Azurite service will run.
    /// </summary>
    [PublicAPI]
    public bool AllServicesEnabled
    {
      get
      {
        return this.EnabledServices.HasFlag(AzuriteServices.All);
      }
    }

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
        waitStrategy = this.EnabledServices.HasFlag(AzuriteServices.Blob) ? waitStrategy.UntilPortIsAvailable(this.BlobContainerPort) : waitStrategy;
        waitStrategy = this.EnabledServices.HasFlag(AzuriteServices.Queue) ? waitStrategy.UntilPortIsAvailable(this.QueueContainerPort) : waitStrategy;
        waitStrategy = this.EnabledServices.HasFlag(AzuriteServices.Table) ? waitStrategy.UntilPortIsAvailable(this.TableContainerPort) : waitStrategy;
        return waitStrategy;
      }
    }

    internal AzuriteServices EnabledServices { get; private set; } = AzuriteServices.All;
  }
}
