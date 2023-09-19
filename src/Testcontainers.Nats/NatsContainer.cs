namespace Testcontainers.Nats;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class NatsContainer : DockerContainer
{
    private readonly NatsConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="NatsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public NatsContainer(NatsConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Nats connection string.
    /// </summary>
    /// <remarks>
    /// If both username and password are set in the builder configuration, they will be included in the connection string.
    /// </remarks>
    /// <returns>A Nats connection string in the format: <c>nats://hostname:port</c>.</returns>
    public string GetConnectionString()
    {
        var endpoint = new UriBuilder("nats://", Hostname, GetMappedPublicPort(NatsBuilder.NatsClientPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        return endpoint.ToString();
    }

    /// <summary>
    /// Gets the Nats monitoring endpoint.
    /// </summary>
    /// <returns>An HTTP address in the format: <c>http://hostname:port</c>.</returns>
    public string GetManagementEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(NatsBuilder.NatsHttpManagementPort)).ToString();
    }
}