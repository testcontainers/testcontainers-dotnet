namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class OracleTestcontainerTest : IClassFixture<OracleFixture>
  {
    private readonly OracleFixture oracleFixture;

    public OracleTestcontainerTest(OracleFixture oracleFixture)
    {
      this.oracleFixture = oracleFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.oracleFixture.Connection;

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void CannotSetDatabase()
    {
      var oracle = new OracleTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => oracle.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var oracle = new OracleTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => oracle.Username = string.Empty);
    }

    [Fact]
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      const string script = @"
        CREATE TABLE MyTable (
          id INT,
          name VARCHAR(30) NOT NULL
        );
        INSERT INTO MyTable (id, name) VALUES (1, 'MyName');
        SELECT * FROM MyTable;
      ";

      // When
      var result = await this.oracleFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(0, result.ExitCode);
      Assert.Contains("MyName", result.Stdout);
    }
  }
}
