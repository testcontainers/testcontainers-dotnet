namespace Testcontainers.CouchDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CouchDbContainer : DockerContainer
{
    private readonly CouchDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public CouchDbContainer(CouchDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the CouchDb connection string.
    /// </summary>
    /// <returns>The CouchDb connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(CouchDbBuilder.CouchDbPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        return endpoint.ToString();
    }
}