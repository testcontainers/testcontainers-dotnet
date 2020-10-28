namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System;
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.Databases;
  using Oracle.ManagedDataAccess.Client;
  using Xunit;

  public class OracleTestcontainerTest : IClassFixture<OracleFixture>
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
      await using var connection = new OracleConnection(this.oracleFixture.Container.ConnectionString);

      // When
      await connection.OpenAsync();

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
