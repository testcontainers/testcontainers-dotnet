namespace DotNet.Testcontainers.Tests.Unit.Linux.MessageBroker
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.MessageBroker;
  using DotNet.Testcontainers.Core.Models.MessageBroker;
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
