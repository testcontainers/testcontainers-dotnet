namespace Testcontainers.Nats;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class NatsContainer : DockerContainer
{
    private readonly NatsConfiguration _natsConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="NatsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public NatsContainer(NatsConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _natsConfig = configuration;
    }

    /// <summary>
    /// Gets the nats connection string
    /// </summary>
    /// <returns>A nats connection string in the form: nats://hostname:mappedPort/>.</returns>
    /// <remarks>
    /// If either username or password is set, the connection string will contain the credentials.
    /// </remarks>
    public string GetConnectionString()
    {
        return new UriBuilder("nats", Hostname, GetMappedPublicPort(NatsBuilder.ClientPort))
        {
            UserName = _natsConfig.Username,
            Password = _natsConfig.Password,
        }.ToString();
    }

    /// <summary>
    /// Gets the nats monitor url
    /// </summary>
    /// <returns>A url in the form: http://hostname:mappedPort/>.</returns>
    public string GetMonitorUrl()
    {
        return new UriBuilder("http", Hostname, GetMappedPublicPort(NatsBuilder.MonitoringPort)).ToString();
    }
}