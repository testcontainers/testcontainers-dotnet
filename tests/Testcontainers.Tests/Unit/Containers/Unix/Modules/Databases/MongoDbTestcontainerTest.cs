namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using MongoDB.Bson;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MongoDbTestcontainerTest : IClassFixture<MongoDbFixture>
  {
    private readonly MongoDbFixture mongoDbFixture;

    public MongoDbTestcontainerTest(MongoDbFixture mongoDbFixture)
    {
      this.mongoDbFixture = mongoDbFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.mongoDbFixture.Connection;

      // When
      var result = await connection.RunCommandAsync<BsonDocument>(@"{ ping: 1 }")
        .ConfigureAwait(false);

      // Then
      Assert.Equal(1.0, result["ok"].AsDouble);
    }
  }
}
