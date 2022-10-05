namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString =>
      $"AccountEndpoint=https://{this.Hostname}:{this.Port};AccountKey={this.Password}";

    /// <summary>
    /// Gets a configured HTTP message handler.
    /// </summary>
    public HttpMessageHandler HttpMessageHandler => new UriRewriter(this.Hostname, this.Port);

    /// <summary>
    /// Gets a configured HTTP client.
    /// </summary>
    public HttpClient HttpClient => new HttpClient(this.HttpMessageHandler);

    private sealed class UriRewriter : DelegatingHandler
    {
      private readonly string hostname;

      private readonly int port;

      public UriRewriter(string hostname, int port)
        : base(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true })
      {
        this.hostname = hostname;
        this.port = port;
      }

      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        request.RequestUri = new UriBuilder("https", this.hostname, this.port, request.RequestUri.PathAndQuery).Uri;
        return base.SendAsync(request, cancellationToken);
      }
    }
  }
}
