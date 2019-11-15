namespace DotNet.Testcontainers.Containers.Configurations.MessageBrokers
{
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class RabbitMqTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    public RabbitMqTestcontainerConfiguration() : base("rabbitmq:3.7.21", 5672)
    {
    }

    public override string Username
    {
      get => this.Environments["RABBITMQ_DEFAULT_USER"];
      set => this.Environments["RABBITMQ_DEFAULT_USER"] = value;
    }

    public override string Password
    {
      get => this.Environments["RABBITMQ_DEFAULT_PASS"];
      set => this.Environments["RABBITMQ_DEFAULT_PASS"] = value;
    }

    public override IWaitUntil WaitStrategy => Wait.UntilPortsAreAvailable(this.DefaultPort);
  }
}
