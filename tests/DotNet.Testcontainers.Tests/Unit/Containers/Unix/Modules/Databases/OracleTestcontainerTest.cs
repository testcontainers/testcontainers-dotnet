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
      await using var connection = await this.oracleFixture.GetConnection()
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
    public void CannotSetPassword()
    {
      var oracle = new OracleTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => oracle.Password = string.Empty);
    }
  }
}
