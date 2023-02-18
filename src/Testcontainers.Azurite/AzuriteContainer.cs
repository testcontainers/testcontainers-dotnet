namespace Testcontainers.Azurite
{
  /// <inheritdoc cref="DockerContainer" />
  [PublicAPI]
  public sealed class AzuriteContainer : DockerContainer
  {
    private readonly AzuriteConfiguration configuration;

    /// <summary>
    ///   Initializes a new instance of the <see cref="AzuriteContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public AzuriteContainer(AzuriteConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
      this.configuration = configuration;
    }

    /// <summary>
    ///   Gets the blob container port.
    /// </summary>
    public int BlobContainerPort
    {
      get
      {
        return AzuriteBuilder.DefaultBlobPort;
      }
    }

    /// <summary>
    ///   Gets the queue container port.
    /// </summary>
    public int QueueContainerPort
    {
      get
      {
        return AzuriteBuilder.DefaultQueuePort;
      }
    }

    /// <summary>
    ///   Gets the table container port.
    /// </summary>
    public int TableContainerPort
    {
      get
      {
        return AzuriteBuilder.DefaultTablePort;
      }
    }

    /// <summary>
    ///   Gets the host blob port.
    /// </summary>
    public int BlobPort
    {
      get
      {
        return this.GetMappedPublicPort(this.BlobContainerPort);
      }
    }

    /// <summary>
    ///   Gets the host queue port.
    /// </summary>
    public int QueuePort
    {
      get
      {
        return this.GetMappedPublicPort(this.QueueContainerPort);
      }
    }

    /// <summary>
    ///   Gets the host table port.
    /// </summary>
    public int TablePort
    {
      get
      {
        return this.GetMappedPublicPort(this.TableContainerPort);
      }
    }

    /// <summary>
    ///   Gets the Azurite workspace directory path.
    /// </summary>
    /// <remarks>
    ///   Corresponds to the default workspace directory path.
    /// </remarks>
    public string WorkspaceContainerLocation
    {
      get
      {
        return AzuriteBuilder.DefaultWorkspaceDirectoryPath;
      }
    }

    /// <summary>
    ///   Gets the directory path where to bind the Azurite workspace directory.
    /// </summary>
    [CanBeNull]
    public string WorkspaceLocationBinding
    {
      get
      {
        return this.configuration.Mounts.SingleOrDefault(x => x.Target == AzuriteBuilder.DefaultWorkspaceDirectoryPath)?.Source;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether debug mode is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Writes logs to the workspace directory path.
    ///   Default value is false.
    /// </remarks>
    public bool DebugModeEnabled
    {
      get
      {
        return this.configuration.DebugModeEnabled ?? false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether silent mode is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Default value is false.
    /// </remarks>
    public bool SilentModeEnabled
    {
      get
      {
        return this.configuration.SilentModeEnabled ?? false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether loose mode is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Default value is false.
    /// </remarks>
    public bool LooseModeEnabled
    {
      get
      {
        return this.configuration.LooseModeEnabled ?? false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether skip API version check is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Default value is false.
    /// </remarks>
    public bool SkipApiVersionCheckEnabled
    {
      get
      {
        return this.configuration.SkipApiVersionCheckEnabled ?? false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether product style URL is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Parses storage account name from the URI path, instead of the URI host.
    ///   Default value is false.
    /// </remarks>
    public bool ProductStyleUrlDisabled
    {
      get
      {
        return this.configuration.ProductStyleUrlDisabled ?? false;
      }
    }

    /// <summary>
    ///   Gets the storage connection string.
    /// </summary>
    public string ConnectionString
    {
      get
      {
        return this.GetConnectionString(this.Hostname, this.BlobPort, this.QueuePort, this.TablePort);
      }
    }

    /// <summary>
    ///   Gets the container storage connection string.
    /// </summary>
    public string ContainerConnectionString
    {
      get
      {
        var alias = this.configuration.NetworkAliases?.FirstOrDefault() ?? this.IpAddress;
        return this.GetConnectionString(alias, this.BlobContainerPort, this.QueueContainerPort, this.TableContainerPort);
      }
    }

    private static string GetEndpoint(UriBuilder uriBuilder, int port)
    {
      uriBuilder.Port = port;
      return uriBuilder.ToString();
    }


    private string GetConnectionString(string hostname, int blobPort, int queuePort, int tablePort)
    {
      // https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio#well-known-storage-account-and-key.
      const string accountName = "devstoreaccount1";
      const string accountKey =
        "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

      var endpointBuilder = new UriBuilder("http", hostname, -1, accountName);

      var connectionString = new Dictionary<string, string>
      {
        { "DefaultEndpointsProtocol", endpointBuilder.Scheme },
        { "AccountName", accountName },
        { "AccountKey", accountKey },
        { "BlobEndpoint", GetEndpoint(endpointBuilder, blobPort) },
        { "QueueEndpoint", GetEndpoint(endpointBuilder, queuePort) },
        { "TableEndpoint", GetEndpoint(endpointBuilder, tablePort) },
      };

      return string.Join(";", connectionString.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
  }
}
