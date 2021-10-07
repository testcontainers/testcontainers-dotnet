namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using MongoDB.Driver;

  public sealed class MongoDbFixture : DatabaseFixture<MongoDbTestcontainer, IMongoDatabase>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new MongoDbTestcontainerConfiguration { Username = "mongoadmin", Password = "secret", Database = "admin" }; // https://hub.docker.com/_/mongo

    public MongoDbFixture()
    {
      this.Container = new TestcontainersBuilder<MongoDbTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new MongoClient(this.Container.ConnectionString)
        .GetDatabase(this.Container.Database);
    }

    public override async Task DisposeAsync()
    {
      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
