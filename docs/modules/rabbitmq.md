# RabbitMQ

Here is an example of a pre-configured RabbitMQ [module](https://www.nuget.org/packages/Testcontainers.RabbitMq). In this example, Testcontainers is utilized to launch a RabbitMQ server within an [xUnit.net](https://xunit.net/) test. The purpose is to establish a connection to the server, send a message, and subsequently validate the successful transmission and consumption of the message. The process also ensures that the received message corresponds accurately to the originally sent message.

Before running the test, make sure to install the required dependencies:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.RabbitMq
dotnet add package RabbitMQ.Client
dotnet add package xunit
```

Copy and paste the following code into a new `.cs` test file:

```csharp
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Testcontainers.RabbitMq;
using Xunit;

public sealed class RabbitMqContainerTest : IAsyncLifetime
{
  private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

  public Task InitializeAsync()
  {
    return _rabbitMqContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _rabbitMqContainer.DisposeAsync().AsTask();
  }

  [Fact]
  public void ConsumeMessageFromQueue()
  {
    const string queue = "hello";

    const string message = "Hello World!";

    string actualMessage = null;

    // Signal the completion of message reception.
    EventWaitHandle waitHandle = new ManualResetEvent(false);

    // Create and establish a connection.
    var connectionFactory = new ConnectionFactory();
    connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());
    using var connection = connectionFactory.CreateConnection();

    // Send a message to the channel.
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queue, false, false, false, null);
    channel.BasicPublish(string.Empty, queue, null, Encoding.Default.GetBytes(message));

    // Consume a message from the channel.
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (_, eventArgs) =>
    {
      actualMessage = Encoding.Default.GetString(eventArgs.Body.ToArray());
      waitHandle.Set();
    };

    channel.BasicConsume(queue, true, consumer);
    waitHandle.WaitOne(TimeSpan.FromSeconds(1));

    Assert.Equal(message, actualMessage);
  }
}
```

To execute the tests, use the command `dotnet test` from your terminal.

Read more about the usage of RabbitMQ and .NET [here](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html).
