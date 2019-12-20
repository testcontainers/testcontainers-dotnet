namespace DotNet.Testcontainers.Containers.Modules.Abstractions
{
  using DotNet.Testcontainers.Containers.Configurations;

  /// <summary>
  /// This class represents an extended configured Testcontainer for databases.
  /// </summary>
  public abstract class TestcontainerDatabase : HostedServiceContainer
  {
    protected TestcontainerDatabase(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public string Database { get; set; }
  }
}
