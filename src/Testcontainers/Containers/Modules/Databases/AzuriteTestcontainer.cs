namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents an extended configured Testcontainer for Azurite.
  /// </summary>
  [PublicAPI]
  public sealed class AzuriteTestcontainer : TestcontainersContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal AzuriteTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the host blob port.
    /// </summary>
    [PublicAPI]
    public int BlobPort
      => this.GetMappedPublicPort(this.ContainerBlobPort);

    /// <summary>
    /// Gets the host queue port.
    /// </summary>
    [PublicAPI]
    public int QueuePort
      => this.GetMappedPublicPort(this.ContainerQueuePort);

    /// <summary>
    /// Gets the host table port.
    /// </summary>
    [PublicAPI]
    public int TablePort
      => this.GetMappedPublicPort(this.ContainerTablePort);

    /// <summary>
    /// Gets or sets the container blob port.
    /// </summary>
    [PublicAPI]
    public int ContainerBlobPort { get; set; }

    /// <summary>
    /// Gets or sets the container queue port.
    /// </summary>
    [PublicAPI]
    public int ContainerQueuePort { get; set; }

    /// <summary>
    /// Gets or sets the container table port.
    /// </summary>
    [PublicAPI]
    public int ContainerTablePort { get; set; }

    /// <summary>
    /// Gets the Storage connection string.
    /// </summary>
    [PublicAPI]
    public string ConnectionString
    {
      get
      {
        // https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio#well-known-storage-account-and-key.
        const string accountName = "devstoreaccount1";

        const string accountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        var endpointBuilder = new UriBuilder("http", this.Hostname, -1, accountName);

        IDictionary<string, string> connectionString = new Dictionary<string, string>();
        connectionString.Add("DefaultEndpointsProtocol", endpointBuilder.Scheme);
        connectionString.Add("AccountName", accountName);
        connectionString.Add("AccountKey", accountKey);

        if (this.ContainerBlobPort > 0)
        {
          endpointBuilder.Port = this.BlobPort;
          connectionString.Add("BlobEndpoint", endpointBuilder.ToString());
        }

        if (this.ContainerQueuePort > 0)
        {
          endpointBuilder.Port = this.QueuePort;
          connectionString.Add("QueueEndpoint", endpointBuilder.ToString());
        }

        if (this.ContainerTablePort > 0)
        {
          endpointBuilder.Port = this.TablePort;
          connectionString.Add("TableEndpoint", endpointBuilder.ToString());
        }

        return string.Join(";", connectionString.Select(kvp => $"{kvp.Key}={kvp.Value}"));
      }
    }
  }
}
