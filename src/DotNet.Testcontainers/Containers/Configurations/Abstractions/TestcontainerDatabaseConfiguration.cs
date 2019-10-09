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

    public virtual string Database { get; set; }
  }
}
