namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerMessageBroker" />
  [PublicAPI]
  public sealed class RabbitMqTestcontainer : TestcontainerMessageBroker
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal RabbitMqTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the message broker connection string.
    /// </summary>
    public string ConnectionString
      => $"amqp://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}";
  }
}
