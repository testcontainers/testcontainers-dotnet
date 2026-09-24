namespace Testcontainers.Elasticsearch;

/// <summary>
/// Provides the certificate authority (CA) and HTTP infrastructure required
/// to authenticate against an Elasticsearch container over TLS.
/// </summary>
internal sealed class ElasticsearchAuthenticatedHttpContext : IDisposable
{
    private readonly X509Certificate2 _caCertificate;

    private readonly HttpClientHandler _httpMessageHandler;

    private readonly Uri _connectionString;

    private readonly AuthenticationHeaderValue _authorization;

    private ElasticsearchAuthenticatedHttpContext(X509Certificate2 caCertificate, HttpClientHandler httpMessageHandler, Uri connectionString, AuthenticationHeaderValue authorization)
    {
        _caCertificate = caCertificate;
        _httpMessageHandler = httpMessageHandler;
        _connectionString = connectionString;
        _authorization = authorization;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ElasticsearchAuthenticatedHttpContext" /> class.
    /// </summary>
    /// <param name="elasticsearchContainer">The Elasticsearch container to authenticate against.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the certificate has been read, returning the authenticated context.</returns>
    public static async Task<ElasticsearchAuthenticatedHttpContext> CreateAsync(ElasticsearchContainer elasticsearchContainer, CancellationToken ct)
    {
        var caCertificate = await elasticsearchContainer.GetCertificateAsync(ct)
            .ConfigureAwait(false);

        var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = CertificateValidations.AuthorityIsRoot(caCertificate);

        try
        {
            var connectionString = new Uri(elasticsearchContainer.GetConnectionString());

            var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.UnescapeDataString(connectionString.UserInfo))));

            return new ElasticsearchAuthenticatedHttpContext(caCertificate, httpMessageHandler, connectionString, authorization);
        }
        catch
        {
            // Nothing owns the certificate and handler yet;
            // dispose of them here so they don't leak.
            httpMessageHandler.Dispose();
            caCertificate.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Creates an authenticated <see cref="HttpClient" /> for the Elasticsearch container.
    /// </summary>
    /// <remarks>
    /// The client shares the handler owned by this context, so disposing the
    /// client does not dispose the handler; only disposing this context does.
    /// This lets callers create multiple clients, or one client used by multiple
    /// consumers, from a single certificate and handler.
    /// </remarks>
    /// <returns>An authenticated <see cref="HttpClient" />.</returns>
    public HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient(_httpMessageHandler, false);
        httpClient.BaseAddress = _connectionString;
        httpClient.DefaultRequestHeaders.Authorization = _authorization;
        return httpClient;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _httpMessageHandler.Dispose();
        _caCertificate.Dispose();
    }
}