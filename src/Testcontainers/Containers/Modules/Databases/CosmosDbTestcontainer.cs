namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    public HttpClient HttpClient
    {
      get
      {
        var httpMessageHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true, };

        return new HttpClient(new UrlRewriter(this.Hostname, this.Port, httpMessageHandler));
      }
    }

    public override string ConnectionString =>
      $"AccountEndpoint=https://{this.Hostname}:{this.Port};AccountKey={this.Password}";

    private class UrlRewriter : DelegatingHandler
    {
      private readonly string host;
      private readonly int portNumber;

      internal UrlRewriter(string host, int portNumber, HttpMessageHandler innerHandler)
        : base(innerHandler)
      {
        this.host = host;
        this.portNumber = portNumber;
      }

      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        request.RequestUri = new Uri($"https://{this.host}:{this.portNumber}{request.RequestUri?.PathAndQuery}");
        var response = await base.SendAsync(request, cancellationToken);
        return response;
      }
    }
  }
}
