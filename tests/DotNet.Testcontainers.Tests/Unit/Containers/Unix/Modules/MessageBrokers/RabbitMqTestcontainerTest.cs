namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class RabbitMqTestcontainerTest : IClassFixture<RabbitMqFixture>
  {
    private readonly RabbitMqFixture rabbitMqFixture;

    public RabbitMqTestcontainerTest(RabbitMqFixture rabbitMqFixture)
    {
      this.rabbitMqFixture = rabbitMqFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      using var connection = await this.rabbitMqFixture.GetConnection()
        .ConfigureAwait(false);

      Assert.True(connection.IsOpen);
    }
  }
}
