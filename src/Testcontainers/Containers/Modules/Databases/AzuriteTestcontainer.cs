namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  [PublicAPI]
  public class AzuriteTestcontainer : TestcontainersContainer
  {
    internal AzuriteTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the host Blob port.
    /// </summary>
    [PublicAPI]
    public int BlobPort => this.GetMappedPublicPort(this.ContainerBlobPort);

    /// <summary>
    /// Gets the host Queue port.
    /// </summary>
    [PublicAPI]
    public int QueuePort => this.GetMappedPublicPort(this.ContainerQueuePort);

    /// <summary>
    /// Gets the host Table port.
    /// </summary>
    [PublicAPI]
    public int TablePort => this.GetMappedPublicPort(this.ContainerTablePort);

    /// <summary>
    /// Gets or sets the container Blob port.
    /// </summary>
    [PublicAPI]
    public int ContainerBlobPort { get; set; }

    /// <summary>
    /// Gets or sets the container Queue port.
    /// </summary>
    [PublicAPI]
    public int ContainerQueuePort { get; set; }

    /// <summary>
    /// Gets or sets the container Table port.
    /// </summary>
    [PublicAPI]
    public int ContainerTablePort { get; set; }

    /// <summary>
    /// Gets the Storage connection string.
    /// </summary>
    [PublicAPI]
    public string ConnectionString =>
      $"DefaultEndpointsProtocol=http;" +
      $"AccountName=devstoreaccount1;" +
      $"AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
      (ContainerBlobPort != 0 ? $"BlobEndpoint=http://{this.Hostname}:{this.BlobPort}/devstoreaccount1;" : string.Empty) +
      (ContainerQueuePort != 0 ? $"QueueEndpoint=http://{this.Hostname}:{this.QueuePort}/devstoreaccount1;" : string.Empty) +
      (ContainerTablePort != 0 ? $"TableEndpoint=http://{this.Hostname}:{this.TablePort}/devstoreaccount1;" : string.Empty);
  }
}
