namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents an extended Testcontainer configuration for Azurite.
  /// </summary>
  [PublicAPI]
  public sealed class AzuriteTestcontainerConfiguration
  {
    /// <summary>
    /// Default workspace directory path '/data/'.
    /// </summary>
    [PublicAPI]
    public const string DefaultWorkspaceDirectoryPath = "/data/";

    private const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.18.0";

    private const int DefaultBlobPort = 10000;

    private const int DefaultQueuePort = 10001;

    private const int DefaultTablePort = 10002;

    private AzuriteServices enabledServices = AzuriteServices.All;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainerConfiguration" /> class.
    /// </summary>
    public AzuriteTestcontainerConfiguration()
      : this(AzuriteImage)
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
    /// Azurite services.
    /// </summary>
    [Flags]
    internal enum AzuriteServices
    {
      /// <summary>
      /// The blob service.
      /// </summary>
      Blob = 1,

      /// <summary>
      /// The queue service.
      /// </summary>
      Queue = 2,

      /// <summary>
      /// The table service.
      /// </summary>
      Table = 4,

      /// <summary>
      /// All services.
      /// </summary>
      All = Blob | Queue | Table,
    }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [PublicAPI]
    public string Image { get; }

    /// <summary>
    /// Gets the wait strategy.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Wait.ForUnixContainer" /> as default value.
    /// </remarks>
    [PublicAPI]
    public IWaitForContainerOS WaitStrategy
    {
      get
      {
        var waitStrategy = Wait.ForUnixContainer();
        waitStrategy = this.enabledServices.HasFlag(AzuriteServices.Blob) ? waitStrategy.UntilPortIsAvailable(this.BlobContainerPort) : waitStrategy;
        waitStrategy = this.enabledServices.HasFlag(AzuriteServices.Queue) ? waitStrategy.UntilPortIsAvailable(this.QueueContainerPort) : waitStrategy;
        waitStrategy = this.enabledServices.HasFlag(AzuriteServices.Table) ? waitStrategy.UntilPortIsAvailable(this.TableContainerPort) : waitStrategy;
        return waitStrategy;
      }
    }

    /// <summary>
    /// Gets or sets the host blob port.
    /// </summary>
    /// <remarks>
    /// Bound to the container blob port.
    /// </remarks>
    [PublicAPI]
    public int BlobPort { get; set; }

    /// <summary>
    /// Gets or sets the container blob port.
    /// </summary>
    [PublicAPI]
    public int BlobContainerPort { get; set; }
      = DefaultBlobPort;

    /// <summary>
    /// Gets or sets a value indicating whether the blob service runs standalone or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool BlobServiceOnlyEnabled
    {
      get
      {
        return AzuriteServices.Blob.Equals(this.enabledServices);
      }

      set
      {
        this.enabledServices = value ? AzuriteServices.Blob : AzuriteServices.All;
      }
    }

    /// <summary>
    /// Gets or sets the host queue port.
    /// </summary>
    /// <remarks>
    /// Bound to the container queue port.
    /// </remarks>
    [PublicAPI]
    public int QueuePort { get; set; }

    /// <summary>
    /// Gets or sets the container queue port.
    /// </summary>
    [PublicAPI]
    public int QueueContainerPort { get; set; }
      = DefaultQueuePort;

    /// <summary>
    /// Gets or sets a value indicating whether the queue service runs standalone or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool QueueServiceOnlyEnabled
    {
      get
      {
        return AzuriteServices.Queue.Equals(this.enabledServices);
      }

      set
      {
        this.enabledServices = value ? AzuriteServices.Queue : AzuriteServices.All;
      }
    }

    /// <summary>
    /// Gets or sets the host table port.
    /// </summary>
    /// <remarks>
    /// Bound to the container queue port.
    /// </remarks>
    [PublicAPI]
    public int TablePort { get; set; }

    /// <summary>
    /// Gets or sets the container table port.
    /// </summary>
    [PublicAPI]
    public int TableContainerPort { get; set; }
      = DefaultTablePort;

    /// <summary>
    /// Gets or sets a value indicating whether the table service runs standalone or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool TableServiceOnlyEnabled
    {
      get
      {
        return AzuriteServices.Table.Equals(this.enabledServices);
      }

      set
      {
        this.enabledServices = value ? AzuriteServices.Table : AzuriteServices.All;
      }
    }

    /// <summary>
    /// Gets a value indicating whether all Azurite service are enabled or not.
    /// </summary>
    [PublicAPI]
    public bool AllServicesEnabled
    {
      get
      {
        return AzuriteServices.All.Equals(this.enabledServices);
      }
    }

    /// <summary>
    /// Gets or sets the workspace directory path.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default workspace directory path.
    /// </remarks>
    [PublicAPI]
    [CanBeNull]
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether debug mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Writes logs to the workspace directory path.
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool DebugModeEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether silent mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool SilentModeEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether loose mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool LooseModeEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether skip API version check is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool SkipApiVersionCheckEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether product style URL is enabled or not.
    /// </summary>
    /// <remarks>
    /// Parses storage account name from the URI path, instead of the URI host.
    /// Default value is false.
    /// </remarks>
    [PublicAPI]
    public bool ProductStyleUrlDisabled { get; set; }
  }
}
