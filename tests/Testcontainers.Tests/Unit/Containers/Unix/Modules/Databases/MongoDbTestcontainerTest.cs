namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using MongoDB.Bson;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MongoDbTestcontainerTest : IClassFixture<MongoDbFixture>, IClassFixture<MongoDbNoAuthFixture>
  {
    private readonly MongoDbFixture mongoDbFixture;

    private readonly MongoDbNoAuthFixture mongoDbNoAuthFixture;

    public MongoDbTestcontainerTest(MongoDbFixture mongoDbFixture, MongoDbNoAuthFixture mongoDbNoAuthFixture)
    {
      this.mongoDbFixture = mongoDbFixture;
      this.mongoDbNoAuthFixture = mongoDbNoAuthFixture;
    }

    private MongoDbTestcontainerTest(MongoDbFixture mongoDbFixture)
    {
      _ = mongoDbFixture;
    }

    private MongoDbTestcontainerTest(MongoDbNoAuthFixture mongoDbNoAuthFixture)
    {
      _ = mongoDbNoAuthFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.mongoDbFixture.Connection;

      // When
      var result = await connection.RunCommandAsync<BsonDocument>("{ ping: 1 }")
        .ConfigureAwait(false);

      // Then
      Assert.Equal(1.0, result["ok"].AsDouble);
    }

    [Fact]
    public void ConnectionStringShouldContainAuthInformation()
    {
      Assert.Matches("mongodb:\\/\\/\\w+:\\w+@\\w+:\\d+", this.mongoDbFixture.Container.ConnectionString);
    }

    [Fact]
    public void ConnectionStringShouldNotContainAuthInformation()
    {
      Assert.Matches("mongodb:\\/\\/\\w+:\\d+", this.mongoDbNoAuthFixture.Container.ConnectionString);
    }

    [Fact]
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      var script = @$"
        db = db.getSiblingDB('{this.mongoDbFixture.Container.Database}')
        db.collection.insertOne({{x:1}});
        print(db.collection.find({{}}));
        print(db.adminCommand('listDatabases'));
      ";

      // When
      var result = await this.mongoDbFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(0, result.ExitCode);
      Assert.Contains(this.mongoDbFixture.Container.Database, result.Stdout);
    }
  }
}
