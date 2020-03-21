namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Threading.Tasks;
  using Fixtures;
  using Oracle.ManagedDataAccess.Client;
  using Xunit;

  public class OracleDbTestcontainerTest : IClassFixture<OracleDbFixture>
  {
    private readonly OracleDbFixture oracleDbFixture;

    public OracleDbTestcontainerTest(OracleDbFixture oracleDbFixture)
    {
      this.oracleDbFixture = oracleDbFixture;
    }

    [Fact]
    public void CanQueryContainerUsingProvidedConnectionString()
    {
      // Act
      var connectionString = this.oracleDbFixture.OracleTestContainer.ConnectionString;

      // Assert
      Assert.NotNull(connectionString);
    }

    [Fact]
    public async Task CanConnectToOracleContainerAndOpenConnection()
    {
      // Arrange
      await using var connection = new OracleConnection(this.oracleDbFixture.OracleTestContainer.ConnectionString);

      // Act
      await connection.OpenAsync();

      // Assert
      Assert.NotEmpty(connection.HostName);
    }
  }
}
