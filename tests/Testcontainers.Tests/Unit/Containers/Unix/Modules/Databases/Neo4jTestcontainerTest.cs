namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public class Neo4jTestcontainerTest : IClassFixture<Neo4jFixture>
  {
    private readonly Neo4jFixture neo4jFixture;

    public Neo4jTestcontainerTest(Neo4jFixture neo4jFixture)
    {
      this.neo4jFixture = neo4jFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      var exception = await Record.ExceptionAsync(() => this.neo4jFixture.Connection.VerifyConnectivityAsync())
        .ConfigureAwait(false);

      Assert.Null(exception);
    }
  }
}
