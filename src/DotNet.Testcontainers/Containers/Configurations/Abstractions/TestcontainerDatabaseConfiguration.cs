namespace DotNet.Testcontainers.Containers.Configurations.Abstractions
{
  using DotNet.Testcontainers.Services;

  /// <summary>
  /// This class represents an extended Testcontainer configuration for databases.
  /// </summary>
  public abstract class TestcontainerDatabaseConfiguration : HostedServiceConfiguration
  {
    protected TestcontainerDatabaseConfiguration(string image, int defaultPort) : base(image, defaultPort, TestcontainersNetworkService.GetAvailablePort())
    {
    }

    protected TestcontainerDatabaseConfiguration(string image, int defaultPort, int port) : base(image, defaultPort, port)
    {
    }

    public virtual string Database { get; set; }
  }
}
