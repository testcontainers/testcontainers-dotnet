namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public sealed class CouchDbTestcontainer : TestcontainerDatabase
  {
    internal CouchDbTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"http://{this.Username}:{this.Password}@localhost:{this.Port}";
  }
}
