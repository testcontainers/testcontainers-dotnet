namespace DotNet.Testcontainers.Containers.Configurations.Abstractions
{
  /// <summary>
  /// This class represents an extended Testcontainer configuration for message brokers.
  /// </summary>
  public abstract class TestcontainerMessageBrokerConfiguration : HostedServiceConfiguration
  {
    protected TestcontainerMessageBrokerConfiguration(string image, int defaultPort, int port = 0) : base(image, defaultPort, port)
    {
    }
  }
}
