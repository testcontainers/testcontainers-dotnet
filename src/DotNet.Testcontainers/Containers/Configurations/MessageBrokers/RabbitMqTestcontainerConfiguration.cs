namespace DotNet.Testcontainers.Containers.Configurations.MessageBrokers
{
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class RabbitMqTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    private const string RabbitMqImage = "rabbitmq:3.7.21";

    private const int RabbitMqPort = 5672;

    public RabbitMqTestcontainerConfiguration()
      : this(RabbitMqImage)
    {
    }

    public RabbitMqTestcontainerConfiguration(string image)
      : base(image, RabbitMqPort)
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

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
