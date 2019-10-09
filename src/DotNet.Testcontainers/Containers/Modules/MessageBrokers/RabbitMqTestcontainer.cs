namespace DotNet.Testcontainers.Containers.Modules.MessageBrokers
{
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public sealed class RabbitMqTestcontainer : TestcontainerMessageBroker
  {
    internal RabbitMqTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"amqp://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}";
  }
}
