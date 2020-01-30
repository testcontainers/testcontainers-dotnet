namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.MessageBroker
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.MessageBrokers;
  using DotNet.Testcontainers.Containers.Modules.MessageBrokers;
  using RabbitMQ.Client;
  using Xunit;

  public class RabbitMqTestcontainerTest
  {
    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<RabbitMqTestcontainer>()
        .WithMessageBroker(new RabbitMqTestcontainerConfiguration
        {
          Username = "rabbitmq",
          Password = "rabbitmq",
        });

      // When
      // Then
      await using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        var factory = new ConnectionFactory { Uri = new Uri(testcontainer.ConnectionString) };

        using (var connection = factory.CreateConnection())
        {
          Assert.True(connection.IsOpen);
        }
      }
    }
  }
}
