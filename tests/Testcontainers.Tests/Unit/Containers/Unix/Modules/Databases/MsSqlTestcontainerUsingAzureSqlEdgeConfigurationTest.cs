namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class MsSqlTestcontainerUsingAzureSqlEdgeConfigurationTest : IClassFixture<AzureSqlEdgeFixture>
  {
    private readonly AzureSqlEdgeFixture azureSqlFixture;

    public MsSqlTestcontainerUsingAzureSqlEdgeConfigurationTest(AzureSqlEdgeFixture azureSqlFixture)
    {
      this.azureSqlFixture = azureSqlFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.azureSqlFixture.Connection;

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }
  }
}
