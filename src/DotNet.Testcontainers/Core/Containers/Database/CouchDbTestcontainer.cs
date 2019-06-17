namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public class CouchDbTestcontainer : DatabaseContainer
  {
    internal CouchDbTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"http://{this.Username}:{this.Password}@localhost:{this.Port}";
  }
}
