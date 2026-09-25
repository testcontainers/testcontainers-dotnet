namespace Testcontainers.Elasticsearch;

/// <summary>
/// Provides authenticated <see cref="HttpClient" /> instances
/// for an Elasticsearch container over TLS.
/// </summary>
internal sealed class ElasticsearchHttpClientFactory : IDisposable
{
    private readonly Uri _connectionString;

    private readonly HttpClientHandler _httpClientHandler;

    private readonly AuthenticationHeaderValue _authorization;

    private readonly X509Certificate2 _caCertificate;

    private ElasticsearchHttpClientFactory(
        Uri connectionString,
        HttpClientHandler httpClientHandler,
        AuthenticationHeaderValue authorization,
        X509Certificate2 caCertificate)
    {
        _connectionString = connectionString;
        _httpClientHandler = httpClientHandler;
        _authorization = authorization;
        _caCertificate = caCertificate;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ElasticsearchHttpClientFactory" /> class.
    /// </summary>
    /// <param name="elasticsearchContainer">The Elasticsearch container to authenticate against.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the certificate has been read, returning the client factory.</returns>
    public static async Task<ElasticsearchHttpClientFactory> CreateAsync(
        ElasticsearchContainer elasticsearchContainer,
        CancellationToken ct)
    {
        var caCertificate = await elasticsearchContainer.GetCertificateAsync(ct)
            .ConfigureAwait(false);

        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = CertificateValidations.AuthorityIsRoot(caCertificate);

        try
        {
            var connectionString = new Uri(elasticsearchContainer.GetConnectionString());

            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.UnescapeDataString(connectionString.UserInfo)));

            var authorization = new AuthenticationHeaderValue("Basic", parameter);

            return new ElasticsearchHttpClientFactory(connectionString, httpClientHandler, authorization, caCertificate);
        }
        catch
        {
            // Nothing owns the certificate and handler yet;
            // dispose of them here so they don't leak.
            httpClientHandler.Dispose();
            caCertificate.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Creates an authenticated <see cref="HttpClient" /> for the Elasticsearch container.
    /// </summary>
    /// <remarks>
    /// The client shares the handler owned by this factory, so disposing the
    /// client does not dispose the handler; only disposing this factory does.
    /// This lets callers create multiple clients, or one client used by multiple
    /// consumers, from a single certificate and handler.
    /// </remarks>
    /// <returns>An authenticated <see cref="HttpClient" />.</returns>
    public HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient(_httpClientHandler, false);
        httpClient.BaseAddress = _connectionString;
        httpClient.DefaultRequestHeaders.Authorization = _authorization;
        return httpClient;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _httpClientHandler.Dispose();
        _caCertificate.Dispose();
    }
}