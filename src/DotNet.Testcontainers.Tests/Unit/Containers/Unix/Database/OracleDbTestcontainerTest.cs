namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using Fixtures;
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
  }
}
