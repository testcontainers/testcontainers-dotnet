namespace Testcontainers.ActiveMq;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ArtemisContainer : DockerContainer
{
    private readonly ActiveMqConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtemisContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ArtemisContainer(ActiveMqConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the ActiveMq broker address.
    /// </summary>
    /// <returns>The ActiveMq broker address.</returns>
    public string GetBrokerAddress()
    {
        var endpoint = new UriBuilder("tcp", Hostname, GetMappedPublicPort(ArtemisBuilder.ArtemisMainPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        return endpoint.ToString();
    }
}