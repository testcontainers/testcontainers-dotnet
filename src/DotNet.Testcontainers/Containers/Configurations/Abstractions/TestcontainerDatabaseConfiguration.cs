namespace DotNet.Testcontainers.Containers.Configurations.Abstractions
{
  /// <summary>
  /// This class represents an extended Testcontainer configuration for databases.
  /// </summary>
  public abstract class TestcontainerDatabaseConfiguration : HostedServiceConfiguration
  {
    protected TestcontainerDatabaseConfiguration(string image, int defaultPort, int port = 0) : base(image, defaultPort, port)
    {
    }

    public virtual string Database { get; set; }
  }
}
