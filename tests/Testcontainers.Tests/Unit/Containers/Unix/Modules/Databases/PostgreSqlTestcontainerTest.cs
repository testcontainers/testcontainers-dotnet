namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class PostgreSqlTestcontainerTest : IClassFixture<PostgreSqlFixture>
  {
    private readonly PostgreSqlFixture postgreSqlFixture;

    public PostgreSqlTestcontainerTest(PostgreSqlFixture postgreSqlFixture)
    {
      this.postgreSqlFixture = postgreSqlFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.postgreSqlFixture.Connection;

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      const string script = @"
        CREATE TABLE MyTable (
	        id serial PRIMARY KEY,
	        name VARCHAR ( 50 ) UNIQUE NOT NULL
        );
        INSERT INTO MyTable (name) VALUES ('MyName');
        SELECT * FROM MyTable;
      ";

      // When
      var result = await this.postgreSqlFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(0, result.ExitCode);
      Assert.Contains("MyName", result.Stdout);
    }
  }
}
