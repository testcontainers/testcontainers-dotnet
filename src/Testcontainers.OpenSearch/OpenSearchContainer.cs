namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OpenSearchContainer : DockerContainer
{
    private readonly OpenSearchConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearch" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public OpenSearchContainer(OpenSearchConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the OpenSearch credentials.
    /// </summary>
    /// <returns>The OpenSearch credentials.</returns>
    public NetworkCredential GetCredentials()
    {
        return new NetworkCredential(_configuration.Username, _configuration.Password);
    }

    /// <summary>
    /// Gets the OpenSearch connection string.
    /// </summary>
    /// <returns>The OpenSearch connection string.</returns>
    public string GetConnectionString()
    {
        var schema = _configuration.TlsEnabled.GetValueOrDefault()
            ? Uri.UriSchemeHttps
            : Uri.UriSchemeHttp;
        return new UriBuilder(
            schema,
            Hostname,
            GetMappedPublicPort(OpenSearchBuilder.OpenSearchRestApiPort)
        ).ToString();
    }
}
