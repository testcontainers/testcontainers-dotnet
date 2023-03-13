using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CosmosDbContainer : DockerContainer
{
    private readonly CosmosDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public CosmosDbContainer(CosmosDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the CosmosDb connection string.
    /// </summary>
    /// <returns>The CosmosDb connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("AccountEndpoint", new UriBuilder("https", Hostname, GetMappedPublicPort(CosmosDbBuilder.CosmosDbPort)).ToString());
        properties.Add("AccountKey", CosmosDbBuilder.DefaultAccountKey);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
    
    /// <summary>
    /// Gets a configured HTTP message handler that automatically trusts the CosmosDb Emulator's certificate.
    /// </summary>
    public HttpMessageHandler HttpMessageHandler => new UriRewriter(Hostname, GetMappedPublicPort(CosmosDbBuilder.CosmosDbPort));

    /// <summary>
    /// Gets a configured HTTP client that automatically trusts the CosmosDb Emulator's certificate.
    /// </summary>
    public HttpClient HttpClient => new HttpClient(HttpMessageHandler);

    private sealed class UriRewriter : DelegatingHandler
    {
        private readonly string _hostname;
        private readonly int _port;

        public UriRewriter(string hostname, int port)
            : base(new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true })
        {
            _hostname = hostname;
            _port = port;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.RequestUri = new UriBuilder("https", _hostname, _port, request.RequestUri.PathAndQuery).Uri;
            return base.SendAsync(request, cancellationToken);
        }
    }
}