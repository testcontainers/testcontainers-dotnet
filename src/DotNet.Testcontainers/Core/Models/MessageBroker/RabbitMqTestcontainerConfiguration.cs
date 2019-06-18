namespace DotNet.Testcontainers.Core.Models.MessageBroker
{
  public sealed class RabbitMqTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    public RabbitMqTestcontainerConfiguration() : base("rabbitmq:3.7.15", 5672)
    {
    }

    public override string Username
    {
      get => this.environments["RABBITMQ_DEFAULT_USER"];
      set => this.environments["RABBITMQ_DEFAULT_USER"] = value;
    }

    public override string Password
    {
      get => this.environments["RABBITMQ_DEFAULT_PASS"];
      set => this.environments["RABBITMQ_DEFAULT_PASS"] = value;
    }
  }
}
