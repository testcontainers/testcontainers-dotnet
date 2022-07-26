namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MariaDbTestcontainerTest : IClassFixture<MariaDbFixture>
  {
    private readonly MariaDbFixture mariaDbFixture;

    public MariaDbTestcontainerTest(MariaDbFixture mariaDbFixture)
    {
      this.mariaDbFixture = mariaDbFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.mariaDbFixture.Connection;

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
      var result = await this.mariaDbFixture.Container.ExecScriptAsync(script)
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
      var result = await this.mariaDbFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(0, result.ExitCode); // exit code is 0 because MariaDB docker image does not have a proper error handler
      Assert.Contains("ERROR 1064 (42000)", result.Stderr);
    }
  }
}
