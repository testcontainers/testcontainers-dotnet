namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerMessageBroker" />
  [PublicAPI]
  public sealed class KafkaTestcontainer : TestcontainerMessageBroker
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal KafkaTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the bootstrap servers.
    /// </summary>
    public string BootstrapServers
      => $"{this.Hostname}:{this.Port}";
  }
}
