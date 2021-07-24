namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using StackExchange.Redis;

  public sealed class RedisFixture : ModuleFixture<RedisTestcontainer>
  {
    public RedisFixture()
      : base(new TestcontainersBuilder<RedisTestcontainer>()
        .WithDatabase(new RedisTestcontainerConfiguration())
        .Build())
    {
    }

    public Task<ConnectionMultiplexer> GetConnection()
    {
      return ConnectionMultiplexer.ConnectAsync(this.Container.ConnectionString);
    }

    public override Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
