namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MySqlTestcontainerTest : IClassFixture<MySqlFixture>
  {
    private readonly MySqlFixture mySqlFixture;

    public MySqlTestcontainerTest(MySqlFixture mySqlFixture)
    {
      this.mySqlFixture = mySqlFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      await using var connection = await this.mySqlFixture.GetConnection()
        .ConfigureAwait(false);

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }
  }
}
