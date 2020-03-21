namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
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

    [Fact]
    public void CanQueryContainerUsingProvidedConnectionString()
    {
      // Act
      var connectionString = this.oracleFixture.OracleTestcontainer.ConnectionString;

      // Assert
      Assert.NotNull(connectionString);
    }
  }
}
