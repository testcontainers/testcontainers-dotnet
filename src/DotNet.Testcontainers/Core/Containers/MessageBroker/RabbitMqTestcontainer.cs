namespace DotNet.Testcontainers.Core.Containers.MessageBroker
{
  using DotNet.Testcontainers.Core.Models;

  public sealed class RabbitMqTestcontainer : TestcontainerMessageBroker
  {
    internal RabbitMqTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"amqp://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}";
  }
}
