namespace Testcontainers.Azurite;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AzuriteContainer : DockerContainer
{
  private readonly AzuriteConfiguration _configuration;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteContainer" /> class.
  /// </summary>
  /// <param name="configuration">The container configuration.</param>
  /// <param name="logger">The logger.</param>
  public AzuriteContainer(AzuriteConfiguration configuration, ILogger logger)
    : base(configuration, logger)
  {
    _configuration = configuration;
  }

  /// <summary>
  ///   Gets a value indicating which Azurite service is turned on.
  /// </summary>
  /// <remarks>
  ///   Default value is <see cref="AzuriteServices" />.All.
  /// </remarks>
  public AzuriteServices AzuriteServices => _configuration.AzuriteServices ?? AzuriteServices.All;

  /// <summary>
  ///   Gets the blob container port.
  /// </summary>
  public int BlobContainerPort =>
    _configuration.AzuriteServices is AzuriteServices.All or AzuriteServices.Blob
      ? _configuration.BlobPort ?? AzuriteBuilder.DefaultBlobPort
      : 0;

  /// <summary>
  ///   Gets the queue container port.
  /// </summary>
  public int QueueContainerPort =>
    _configuration.AzuriteServices is AzuriteServices.All or AzuriteServices.Queue
      ? _configuration.QueuePort ?? AzuriteBuilder.DefaultQueuePort
      : 0;

  /// <summary>
  ///   Gets the table container port.
  /// </summary>
  public int TableContainerPort =>
    _configuration.AzuriteServices is AzuriteServices.All or AzuriteServices.Table
      ? _configuration.TablePort ?? AzuriteBuilder.DefaultTablePort
      : 0;

  /// <summary>
  ///   Gets the host blob port.
  /// </summary>
  public int BlobPort => BlobContainerPort > 0 ? GetMappedPublicPort(BlobContainerPort) : 0;

  /// <summary>
  ///   Gets the host queue port.
  /// </summary>
  public int QueuePort => QueueContainerPort > 0 ? GetMappedPublicPort(QueueContainerPort) : 0;

  /// <summary>
  ///   Gets the host table port.
  /// </summary>
  public int TablePort => TableContainerPort > 0 ? GetMappedPublicPort(TableContainerPort) : 0;

  /// <summary>
  ///   Gets the Azurite workspace directory path.
  /// </summary>
  /// <remarks>
  ///   Corresponds to the default workspace directory path.
  /// </remarks>
  public string WorkspaceLocation => _configuration.WorkspaceLocation ?? AzuriteBuilder.DefaultWorkspaceDirectoryPath;

  /// <summary>
  ///   Gets the directory path where to bind the Azurite workspace directory.
  /// </summary>
  [CanBeNull]
  public string WorkspaceLocationBinding => _configuration.WorkspaceLocationBinding;

  /// <summary>
  ///   Gets a value indicating whether debug mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Writes logs to the workspace directory path.
  ///   Default value is false.
  /// </remarks>
  public bool DebugModeEnabled => _configuration.DebugModeEnabled ?? false;

  /// <summary>
  ///   Gets a value indicating whether silent mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public bool SilentModeEnabled => _configuration.SilentModeEnabled ?? false;

  /// <summary>
  ///   Gets a value indicating whether loose mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public bool LooseModeEnabled => _configuration.LooseModeEnabled ?? false;

  /// <summary>
  ///   Gets a value indicating whether skip API version check is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public bool SkipApiVersionCheckEnabled => _configuration.SkipApiVersionCheckEnabled ?? false;

  /// <summary>
  ///   Gets a value indicating whether product style URL is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Parses storage account name from the URI path, instead of the URI host.
  ///   Default value is false.
  /// </remarks>
  public bool ProductStyleUrlDisabled => _configuration.ProductStyleUrlDisabled ?? false;

  /// <summary>
  ///   Gets the Storage connection string.
  /// </summary>
  public string ConnectionString
  {
    get
    {
      // https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio#well-known-storage-account-and-key.
      const string accountName = "devstoreaccount1";
      const string accountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

      var endpointBuilder = new UriBuilder("http", Hostname, -1, accountName);

      var connectionString = new Dictionary<string, string>();
      connectionString.Add("DefaultEndpointsProtocol", endpointBuilder.Scheme);
      connectionString.Add("AccountName", accountName);
      connectionString.Add("AccountKey", accountKey);

      if (AzuriteServices is AzuriteServices.All or AzuriteServices.Blob && BlobContainerPort > 0)
      {
        endpointBuilder.Port = GetMappedPublicPort(BlobContainerPort);
        connectionString.Add("BlobEndpoint", endpointBuilder.ToString());
      }

      if (AzuriteServices is AzuriteServices.All or AzuriteServices.Queue && QueueContainerPort > 0)
      {
        endpointBuilder.Port = GetMappedPublicPort(QueueContainerPort);
        connectionString.Add("QueueEndpoint", endpointBuilder.ToString());
      }

      if (AzuriteServices is AzuriteServices.All or AzuriteServices.Table && TableContainerPort > 0)
      {
        endpointBuilder.Port = GetMappedPublicPort(TableContainerPort);
        connectionString.Add("TableEndpoint", endpointBuilder.ToString());
      }

      return string.Join(";", connectionString.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
  }
}