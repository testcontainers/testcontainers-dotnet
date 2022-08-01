namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MsSqlTestcontainerTest : IClassFixture<MsSqlFixture>
  {
    private readonly MsSqlFixture msSqlFixture;

    public MsSqlTestcontainerTest(MsSqlFixture msSqlFixture)
    {
      this.msSqlFixture = msSqlFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.msSqlFixture.Connection;

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Username = string.Empty);
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
      var result = await this.msSqlFixture.Container.ExecScriptAsync(script)
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
      var result = await this.msSqlFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.NotEqual(0, result.ExitCode);
      Assert.Contains("Msg 102, Level 15, State 1", result.Stderr);
    }
  }
}
