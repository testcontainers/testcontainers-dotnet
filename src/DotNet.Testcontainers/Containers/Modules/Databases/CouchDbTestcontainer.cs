namespace DotNet.Testcontainers.Containers.Modules.Databases
{
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public sealed class CouchDbTestcontainer : TestcontainerDatabase
  {
    internal CouchDbTestcontainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"http://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}";
  }
}
