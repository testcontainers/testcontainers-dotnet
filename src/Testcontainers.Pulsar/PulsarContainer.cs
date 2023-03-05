
namespace Testcontainers.Pulsar
{
  /// <inheritdoc cref="DockerContainer" />
  [PublicAPI]
  public sealed class PulsarContainer : DockerContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PulsarContainer(PulsarConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
  }
}
