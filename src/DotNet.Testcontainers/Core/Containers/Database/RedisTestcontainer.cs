namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public sealed class RedisTestcontainer : TestcontainerDatabase
  {
    internal RedisTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"{this.Hostname}:{this.Port}";
  }
}
