namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class RabbitMqTestcontainerTest : IClassFixture<RabbitMqFixture>
  {
    private readonly RabbitMqFixture rabbitMqFixture;

    public RabbitMqTestcontainerTest(RabbitMqFixture rabbitMqFixture)
    {
      this.rabbitMqFixture = rabbitMqFixture;
    }

    [Fact]
    public void ConnectionEstablished()
    {
      Assert.True(this.rabbitMqFixture.Connection.IsOpen);
    }
  }
}
