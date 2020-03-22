namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Oracle.ManagedDataAccess.Client;
  using Xunit;

  public class OracleTestcontainerTest : IClassFixture<OracleFixture>
  {
    private readonly OracleFixture oracleFixture;

    public OracleTestcontainerTest(OracleFixture oracleFixture)
    {
      this.oracleFixture = oracleFixture;
    }

    [Theory]
    [InlineData("localhost", "system")]
    public void CanQueryContainerUsingProvidedConnectionString(string database, string userName)
    {
      // Act
      var actualDatabase = this.oracleFixture.OracleTestcontainer.Database;
      var actualUserName = this.oracleFixture.OracleTestcontainer.Username;

      // Assert
      Assert.Equal(database, actualDatabase);
      Assert.Equal(userName, actualUserName);
    }

    [Fact]
    public async Task CanConnectToOracleContainerAndOpenConnection()
    {
      // Arrange
      await using var connection = new OracleConnection(this.oracleFixture.OracleTestcontainer.ConnectionString);

      // Act
      await connection.OpenAsync();

      // Assert
      Assert.Equal(ConnectionState.Open ,connection.State);
    }
  }
}
