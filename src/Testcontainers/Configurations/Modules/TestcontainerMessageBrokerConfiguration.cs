namespace DotNet.Testcontainers.Configurations
{
  /// <summary>
  /// This class represents an extended Testcontainer configuration for message brokers.
  /// </summary>
  public abstract class TestcontainerMessageBrokerConfiguration : HostedServiceConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainerMessageBrokerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <param name="defaultPort">The container port.</param>
    /// <param name="port">The host port.</param>
    protected TestcontainerMessageBrokerConfiguration(string image, int defaultPort, int port = 0)
      : base(image, defaultPort, port)
    {
    }
  }
}
