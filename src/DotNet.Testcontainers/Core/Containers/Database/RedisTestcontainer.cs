namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public class RedisTestcontainer : DatabaseContainer
  {
    internal RedisTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"{this.Hostname}:{this.Port}";
  }
}
