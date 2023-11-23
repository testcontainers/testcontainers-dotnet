namespace Testcontainers.ActiveMq;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ActiveMqContainer : DockerContainer
{
    private readonly ActiveMqConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public ActiveMqContainer(ActiveMqConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the broker address.
    /// </summary>
    /// <returns>The broker address.</returns>
    public string GetBrokerAddress()
    {
        return new UriBuilder("tcp://", Hostname, GetMappedPublicPort(ActiveMqBuilder.ActiveMqMainPort))
        {
            UserName = Uri.EscapeDataString(_configuration.Username),
            Password = Uri.EscapeDataString(_configuration.Password)
        }.ToString();
    }
}