namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Models;

  public abstract class DatabaseContainer : TestcontainersContainer
  {
    protected DatabaseContainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public virtual string Hostname { get; set; }

    public virtual string Port { get; set; }

    public virtual string Database { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public abstract string ConnectionString { get; }
  }
}
