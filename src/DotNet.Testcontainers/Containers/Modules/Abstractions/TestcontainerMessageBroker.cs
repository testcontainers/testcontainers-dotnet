namespace DotNet.Testcontainers.Containers.Modules.Abstractions
{
  using DotNet.Testcontainers.Containers.Configurations;

  /// <summary>
  /// This class represents an extended configured and created Testcontainer for message brokers.
  /// </summary>
  public abstract class TestcontainerMessageBroker : HostedServiceContainer
  {
    protected TestcontainerMessageBroker(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }
  }
}
