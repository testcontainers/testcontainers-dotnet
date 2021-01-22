namespace DotNet.Testcontainers.Containers.Modules.Abstractions
{
  using DotNet.Testcontainers.Containers.Configurations;

  /// <summary>
  /// This class represents an extended configured and created Testcontainer. It is convenient for common Testcontainers to provide a module with all necessary properties, which does not exist in <see cref="TestcontainersContainer" />.
  /// </summary>
  public abstract class HostedServiceContainer : TestcontainersContainer
  {
    protected HostedServiceContainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public int Port => this.GetMappedPublicPort(this.ContainerPort);

    public virtual int ContainerPort { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }
  }
}
