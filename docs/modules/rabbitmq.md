# RabbitMQ

Here is an example of a pre-configured RabbitMQ [module](https://www.nuget.org/packages/Testcontainers.RabbitMq;).
In this example, Testcontainers is utilized to launch a RabbitMQ server within an [xUnit.net][xunit] test. The purpose
is to establish a connection to the server, send a message, and subsequently validate the successful transmission and
consumption of the message. The process also ensures that the received message corresponds accurately to the
originally sent message.

```csharp
using RabbitMQ.Client;
using System.Text;
using Testcontainers.RabbitMq;
using Xunit;


public class RabbitMqContainerTest : IAsyncLifetime
{
  private RabbitMqContainer _rabbitMqContainer;

  public Task InitializeAsync()
  {
    _rabbitMqContainer = new RabbitMqBuilder().Build();
    return _rabbitMqContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _rabbitMqContainer.DisposeAsync().AsTask();
  }

  [Fact]
  public void UnitTest()
  {
    // Create connection
    var connectionFactory = new ConnectionFactory();
    connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());

    using var connection = connectionFactory.CreateConnection();

    Assert.True(connection.IsOpen);

    // Send message
    var channel = connection.CreateModel();
    var queueName = "my_queue";
    var message = "Hello, RabbitMQ!";

    channel.QueueDeclare(queueName, false, false, false, null);
    channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(message));

    // Consume message
    var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
    var receivedMessage = "";

    consumer.Received += (_, eventArgs) =>
    {
        var body = eventArgs.Body.ToArray();
        receivedMessage = Encoding.UTF8.GetString(body);
    };

    channel.BasicConsume(queueName, true, consumer);
    Thread.Sleep(1000); // to avoid race condition
    Assert.Equal(message, receivedMessage);
  }

}
```

Make sure to install the [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client) package to be able to connect to RabbitMQ.
Read more about the usage of RabbitMQ and .NET [here](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html).


[xunit]: https://xunit.net/
