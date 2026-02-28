namespace Testcontainers.Kafka;

/// <summary>
/// Provides the Kafka connection string.
/// </summary>
internal sealed class KafkaConnectionStringProvider : ContainerConnectionStringProvider<KafkaContainer, KafkaConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBootstrapAddress();
    }
}