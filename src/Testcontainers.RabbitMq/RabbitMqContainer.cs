namespace Testcontainers.RabbitMq;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RabbitMqContainer : DockerContainer
{
    private readonly RabbitMqConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public RabbitMqContainer(RabbitMqConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the RabbitMq connection string.
    /// </summary>
    /// <returns>The RabbitMq connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint = new UriBuilder(
            "amqp",
            Hostname,
            GetMappedPublicPort(RabbitMqBuilder.RabbitMqPort)
        );
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        return endpoint.ToString();
    }
}
