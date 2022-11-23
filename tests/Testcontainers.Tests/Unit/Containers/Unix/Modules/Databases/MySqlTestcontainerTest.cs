namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class MySqlTestcontainerTest
  {
    public sealed class MySqlCustomUsernameTest : IClassFixture<MySqlFixture>
    {
      private readonly MySqlFixture mySqlFixture;

      public MySqlCustomUsernameTest(MySqlFixture mySqlFixture)
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

    public sealed class MySqlRootUsernameTest : IClassFixture<MySqlRootUsernameFixture>
    {
      private readonly MySqlFixture mySqlFixture;

      public MySqlRootUsernameTest(MySqlRootUsernameFixture mySqlFixture)
      {
        this.mySqlFixture = mySqlFixture;
      }

      [Fact]
      public Task ConnectionEstablished()
      {
        return new MySqlCustomUsernameTest(this.mySqlFixture).ConnectionEstablished();
      }

      [Fact]
      public Task ExecScriptInRunningContainer()
      {
        return new MySqlCustomUsernameTest(this.mySqlFixture).ExecScriptInRunningContainer();
      }

      [Fact]
      public Task ThrowErrorInRunningContainerWithInvalidScript()
      {
        return new MySqlCustomUsernameTest(this.mySqlFixture).ThrowErrorInRunningContainerWithInvalidScript();
      }
    }
  }
}
