namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MySqlTestcontainerTest : IClassFixture<MySqlFixture>
  {
    private readonly MySqlFixture mySqlFixture;

    public MySqlTestcontainerTest(MySqlFixture mySqlFixture)
    {
      this.mySqlFixture = mySqlFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.mySqlFixture.Connection;

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
        id INT(6) UNSIGNED PRIMARY KEY,
        name VARCHAR(30) NOT NULL
        );
        GO
        INSERT INTO MyTable (id, name) VALUES (1, 'MyName');
        GO
        SELECT * FROM MyTable;
        ";

      // When
      var results = await this.mySqlFixture.Container.ExecScriptAsync(script);

      // Then
      Assert.Contains("MyName", results.Stdout);
    }

    [Fact]
    public async Task ThrowErrorInRunningContainerWithInvalidScript()
    {
      // Given
      const string script = "invalid SQL command";

      // When
      var results = await this.mySqlFixture.Container.ExecScriptAsync(script);

      // Then
      Assert.NotEqual(0, results.ExitCode);
      Assert.NotEqual(string.Empty, results.Stderr);
    }
  }
}
