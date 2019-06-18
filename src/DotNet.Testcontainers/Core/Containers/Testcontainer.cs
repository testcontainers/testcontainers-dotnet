namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Models;

  public abstract class Testcontainer : TestcontainersContainer
  {
    protected Testcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public virtual int Port { get; set; }

    public virtual string Hostname { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public abstract string ConnectionString { get; }
  }
}
