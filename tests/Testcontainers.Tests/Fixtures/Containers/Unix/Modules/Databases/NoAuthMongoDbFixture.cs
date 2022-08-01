namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using MongoDB.Driver;

  public sealed class NoAuthMongoDbFixture : DatabaseFixture<MongoDbTestcontainer, IMongoDatabase>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new MongoDbTestcontainerConfiguration { Username = null, Password = null, Database = "admin" }; // https://hub.docker.com/_/mongo

    public NoAuthMongoDbFixture()
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
