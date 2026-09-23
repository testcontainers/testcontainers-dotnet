namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ElasticsearchContainer : DockerContainer
{
    private readonly ElasticsearchConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ElasticsearchContainer(ElasticsearchConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Elasticsearch connection string.
    /// </summary>
    /// <returns>The Elasticsearch connection string.</returns>
    public string GetConnectionString()
    {
        var scheme = _configuration.TlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        var endpoint = new UriBuilder(scheme, Hostname, GetMappedPublicPort(ElasticsearchBuilder.ElasticsearchHttpsPort));
        endpoint.UserName = _configuration.Username;
        endpoint.Password = _configuration.Password;
        return endpoint.ToString();
    }

    /// <summary>
    /// Gets the Elasticsearch OTLP endpoint.
    /// </summary>
    /// <remarks>
    /// Elasticsearch 9.5 and later accept OTLP over HTTP at <c>/_otlp/v1/logs</c>,
    /// <c>/_otlp/v1/metrics</c> and <c>/_otlp/v1/traces</c>. In contrast to the
    /// connection string, the endpoint does not contain the credentials. Clients
    /// must send them in the <c>Authorization</c> header.
    /// </remarks>
    /// <returns>The Elasticsearch OTLP endpoint.</returns>
    public string GetOtlpEndpoint()
    {
        const string otlpPath = "/_otlp/";
        var scheme = _configuration.TlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        return new UriBuilder(scheme, Hostname, GetMappedPublicPort(ElasticsearchBuilder.ElasticsearchHttpsPort), otlpPath).ToString();
    }

    /// <summary>
    /// Gets the Elasticsearch HTTP certificate authority (CA).
    /// </summary>
    /// <remarks>
    /// Elasticsearch generates a self-signed certificate authority (CA) during the
    /// startup that signs the HTTP certificate. Configure the client to trust it
    /// instead of accepting any certificate. The certificate is only available for
    /// Elasticsearch 8.0 and later with TLS enabled.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the certificate has been read, returning the Elasticsearch HTTP certificate authority (CA).</returns>
    public async Task<X509Certificate2> GetCertificateAsync(CancellationToken ct = default)
    {
        var certificateBytes = await ReadFileAsync(ElasticsearchBuilder.ElasticsearchHttpCaCertificateFilePath, ct)
            .ConfigureAwait(false);

#if NET5_0_OR_GREATER
        return X509Certificate2.CreateFromPem(Encoding.Default.GetString(certificateBytes));
#else
        return new X509Certificate2(certificateBytes);
#endif
    }
}