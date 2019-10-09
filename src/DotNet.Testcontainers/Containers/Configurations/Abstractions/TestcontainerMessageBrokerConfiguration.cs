namespace DotNet.Testcontainers.Containers.Configurations.Abstractions
{
  using DotNet.Testcontainers.Services;

  /// <summary>
  /// This class represents an extended Testcontainer configuration for message brokers.
  /// </summary>
  public abstract class TestcontainerMessageBrokerConfiguration : HostedServiceConfiguration
  {
    protected TestcontainerMessageBrokerConfiguration(string image, int defaultPort) : base(image, defaultPort, TestcontainersNetworkService.GetAvailablePort())
    {
    }
  }
}
