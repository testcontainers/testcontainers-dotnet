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
    /// <param name="logger">The logger.</param>
    public ElasticsearchContainer(ElasticsearchConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Elasticsearch connection string.
    /// </summary>
    /// <remarks>
    /// The Elasticsearch module does not export the SSL certificate from the container by default.
    /// If you are trying to connect to the Elasticsearch service, you need to override the certificate validation callback to establish the connection.
    /// We will export the certificate and support trusted SSL connections in the future.
    /// </remarks>
    /// <returns>The Elasticsearch connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint = new UriBuilder(Uri.UriSchemeHttps, Hostname, GetMappedPublicPort(ElasticsearchBuilder.ElasticsearchHttpsPort));
        endpoint.UserName = _configuration.Username;
        endpoint.Password = _configuration.Password;
        return endpoint.ToString();
    }
}