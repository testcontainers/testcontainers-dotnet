namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  [PublicAPI]
  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    public HttpMessageHandler HttpMessageHandler => new UriRewriter(this.Hostname, this.Port);

    public HttpClient HttpClient
    {
      get
      {
        return new HttpClient(new UriRewriter(this.Hostname, this.Port));
      }
    }

    public override string ConnectionString =>
      $"AccountEndpoint=https://{this.Hostname}:{this.Port};AccountKey={this.Password}";

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
