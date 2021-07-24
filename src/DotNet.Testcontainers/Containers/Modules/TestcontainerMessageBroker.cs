namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents an extended configured and created Testcontainer for message brokers.
  /// </summary>
  public abstract class TestcontainerMessageBroker : HostedServiceContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainerMessageBroker" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    protected TestcontainerMessageBroker(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }
  }
}
