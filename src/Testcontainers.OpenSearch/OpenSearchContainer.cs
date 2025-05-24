namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OpenSearchContainer : DockerContainer
{
    private readonly OpenSearchConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public OpenSearchContainer(OpenSearchConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Returns the url which is used to connect to OpenSearch cluster.
    /// </summary>
    /// <returns></returns>
    public string GetConnection()
    {
        return new UriBuilder(
            _configuration.DisabledSecurity == true ? Uri.UriSchemeHttp : Uri.UriSchemeHttps,
            Hostname,
            GetMappedPublicPort(OpenSearchBuilder.OpenSearchHttpApiPort)
        ).ToString();
    }

    /// <summary>
    /// Gets the credentials for OpenSearch cluster connections.
    /// </summary>
    /// <returns></returns> 
    public NetworkCredential GetConnectionCredentials()
    {
        return new NetworkCredential(_configuration.Username, _configuration.Password);
    }
}