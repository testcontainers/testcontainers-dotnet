namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class AzuriteTestcontainerTest : IClassFixture<AzuriteFixture>
  {
    private readonly AzuriteFixture azuriteFixture;

    public AzuriteTestcontainerTest(AzuriteFixture azuriteFixture)
    {
      this.azuriteFixture = azuriteFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.azuriteFixture.Connection;

      // When
      var properties = await connection.GetPropertiesAsync().ConfigureAwait(false);

      // Then
      Assert.True(properties.GetRawResponse().Status is >= 200 and <= 299);
    }
  }
}
