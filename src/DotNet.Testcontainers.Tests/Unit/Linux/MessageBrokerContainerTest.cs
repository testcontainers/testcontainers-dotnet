namespace DotNet.Testcontainers.Tests.Unit.Linux
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.MessageBroker;
  using DotNet.Testcontainers.Core.Models.MessageBroker;
  using RabbitMQ.Client;
  using Xunit;

  public class MessageBrokerContainerTest
  {
    [Fact]
    public async Task RabbitMqContainer()
    {
      // Given
      // When
      var testcontainersBuilder = new TestcontainersBuilder<RabbitMqTestcontainer>()
        .WithMessageBroker(new RabbitMqTestcontainerConfiguration
        {
          Username = "rabbitmq",
          Password = "rabbitmq",
        });

      // Then
      using (var testcontainer = testcontainersBuilder.Build())
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
