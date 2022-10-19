namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class Neo4jTestcontainerTest : IClassFixture<Neo4jFixture>
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

    [Fact]
    public void CannotSetDatabase()
    {
      var neo4j = new Neo4jTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => neo4j.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var neo4j = new Neo4jTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => neo4j.Username = string.Empty);
    }
  }
}
