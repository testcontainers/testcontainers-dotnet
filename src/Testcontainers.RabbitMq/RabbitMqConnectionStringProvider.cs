namespace Testcontainers.RabbitMq;

/// <summary>
/// Provides the RabbitMq connection string.
/// </summary>
internal sealed class RabbitMqConnectionStringProvider : ContainerConnectionStringProvider<RabbitMqContainer, RabbitMqConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}