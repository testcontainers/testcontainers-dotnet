namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
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
    public string ConnectionString
    {
      get
      {
        var connectionStringParts = new List<string>()
        {
          "DefaultEndpointsProtocol=http",
          "AccountName=devstoreaccount1",
          "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==",
        };

        if (this.ContainerBlobPort != 0)
        {
          connectionStringParts.Add(this.GetServiceEndpoint("Blob", this.BlobPort));
        }

        if (this.ContainerQueuePort != 0)
        {
          connectionStringParts.Add(this.GetServiceEndpoint("Queue", this.QueuePort));
        }

        if (this.ContainerTablePort != 0)
        {
          connectionStringParts.Add(this.GetServiceEndpoint("Table", this.TablePort));
        }

        return string.Join(";", connectionStringParts);
      }
    }

    private string GetServiceEndpoint(string service, int port)
    {
      return $"{service}Endpoint=http://{this.Hostname}:{port}/devstoreaccount1";
    }
  }
}
