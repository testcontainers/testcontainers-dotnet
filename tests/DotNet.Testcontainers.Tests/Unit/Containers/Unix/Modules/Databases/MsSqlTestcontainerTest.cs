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
      await using var connection = await this.msSqlFixture.GetConnection()
        .ConfigureAwait(false);

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void CannotSetDatabase()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Username = string.Empty);
    }
  }
}
