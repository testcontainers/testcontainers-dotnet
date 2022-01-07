namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class Db2TestcontainerTest : IClassFixture<Db2Fixture>
  {
    private readonly Db2Fixture db2Fixture;

    public Db2TestcontainerTest(Db2Fixture db2Fixture)
    {
      this.db2Fixture = db2Fixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.db2Fixture.Connection;

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }
  }
}
