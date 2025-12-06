namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CosmosDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public CosmosDbContainer(CosmosDbConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the CosmosDb connection string.
    /// </summary>
    /// <returns>The CosmosDb connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("AccountEndpoint", new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(CosmosDbBuilder.CosmosDbPort)).ToString());
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

    /// <summary>
    /// Rewrites the HTTP requests to target the running CosmosDb Emulator instance.
    /// </summary>
    private sealed class UriRewriter : DelegatingHandler
    {
        private readonly string _hostname;

        private readonly ushort _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriRewriter" /> class.
        /// </summary>
        /// <param name="hostname">The target hostname.</param>
        /// <param name="port">The target port.</param>
        public UriRewriter(string hostname, ushort port)
            : base(new HttpClientHandler())
        {
            _hostname = hostname;
            _port = port;
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.RequestUri = new UriBuilder(Uri.UriSchemeHttp, _hostname, _port, request.RequestUri.PathAndQuery).Uri;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
