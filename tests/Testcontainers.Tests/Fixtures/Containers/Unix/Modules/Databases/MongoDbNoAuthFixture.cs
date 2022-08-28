namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using MongoDB.Driver;

  [UsedImplicitly]
  public sealed class MongoDbNoAuthFixture : DatabaseFixture<MongoDbTestcontainer, IMongoDatabase>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new MongoDbTestcontainerConfiguration { Database = "db", Username = null, Password = null };

    public MongoDbNoAuthFixture()
    {
      this.Container = new TestcontainersBuilder<MongoDbTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
