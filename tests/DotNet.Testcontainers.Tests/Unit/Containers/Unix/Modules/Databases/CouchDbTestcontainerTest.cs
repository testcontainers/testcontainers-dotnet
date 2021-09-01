namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class CouchDbTestcontainerTest : IClassFixture<CouchDbFixture>
  {
    private readonly CouchDbFixture couchDbFixture;

    public CouchDbTestcontainerTest(CouchDbFixture couchDbFixture)
    {
      this.couchDbFixture = couchDbFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var database = this.couchDbFixture.Connection.Database;

      // When
      var response = await database.PutAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(response.IsSuccess);
    }
  }
}
