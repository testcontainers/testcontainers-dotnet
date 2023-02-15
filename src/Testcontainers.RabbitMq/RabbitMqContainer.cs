namespace Testcontainers.RabbitMq;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RabbitMqContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public RabbitMqContainer(RabbitMqConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}