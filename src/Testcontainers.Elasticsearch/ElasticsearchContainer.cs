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
    /// Returns <c>true</c> if https connection to container is enabled.
    /// </summary>
    public bool HttpsEnabled => _configuration.HttpsEnabled;

    /// <summary>
    /// Gets the Elasticsearch credentials.
    /// </summary>
    /// <returns>The Elasticsearch credentials.</returns>
    public NetworkCredential GetCredentials() => new(_configuration.Username, _configuration.Password);

    /// <summary>
    /// Gets the Elasticsearch connection string.
    /// </summary>
    /// <remarks>
    /// The Elasticsearch module does not export the SSL certificate from the container
    /// by default. If you are trying to connect to the Elasticsearch service, you need
    /// to override the certificate validation callback to establish the connection.
    /// We will export the certificate and support trusted SSL connections in the future.
    /// </remarks>
    /// <returns>The Elasticsearch connection string.</returns>
    public string GetConnectionString()
    {
        var scheme = _configuration.HttpsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        var endpoint = new UriBuilder(scheme, Hostname, GetMappedPublicPort(ElasticsearchBuilder.ElasticsearchHttpsPort));
        endpoint.UserName = _configuration.Username;
        endpoint.Password = _configuration.Password;
        return endpoint.ToString();
    }
}