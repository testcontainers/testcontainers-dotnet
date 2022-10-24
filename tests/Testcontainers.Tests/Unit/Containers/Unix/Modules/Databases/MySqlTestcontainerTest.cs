namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public static class MySqlTestcontainerTest
  {
    [Collection(nameof(Testcontainers))]
    public sealed class MySqlTestcontainerRootUserTest : IClassFixture<MySqlRootUserFixture>
    {
      private readonly MySqlRootUserFixture mySqlFixture;

      public MySqlTestcontainerRootUserTest(MySqlRootUserFixture mySqlFixture)
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
        INSERT INTO MyTable (id, name) VALUES (1, 'MyName');
        SELECT * FROM MyTable;
      ";

        // When
        var result = await this.mySqlFixture.Container.ExecScriptAsync(script)
          .ConfigureAwait(false);

        // Then
        Assert.DoesNotContain("ERROR", result.Stderr);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("MyName", result.Stdout);
      }

      [Fact]
      public async Task ThrowErrorInRunningContainerWithInvalidScript()
      {
        // Given
        const string script = "invalid SQL command";

        // When
        var result = await this.mySqlFixture.Container.ExecScriptAsync(script)
          .ConfigureAwait(false);

        // Then
        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("ERROR 1064 (42000)", result.Stderr);
      }
    }

    [Collection(nameof(Testcontainers))]
    public sealed class MySqlTestcontainerNormalUserTest : IClassFixture<MySqlNormalUserFixture>
    {
      private readonly MySqlNormalUserFixture mySqlFixture;

      public MySqlTestcontainerNormalUserTest(MySqlNormalUserFixture mySqlFixture)
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
        INSERT INTO MyTable (id, name) VALUES (1, 'MyName');
        SELECT * FROM MyTable;
      ";

        // When
        var result = await this.mySqlFixture.Container.ExecScriptAsync(script)
          .ConfigureAwait(false);

        // Then
        Assert.DoesNotContain("ERROR", result.Stderr);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("MyName", result.Stdout);
      }

      [Fact]
      public async Task ThrowErrorInRunningContainerWithInvalidScript()
      {
        // Given
        const string script = "invalid SQL command";

        // When
        var result = await this.mySqlFixture.Container.ExecScriptAsync(script)
          .ConfigureAwait(false);

        // Then
        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("ERROR 1064 (42000)", result.Stderr);
      }
    }
  }
}
