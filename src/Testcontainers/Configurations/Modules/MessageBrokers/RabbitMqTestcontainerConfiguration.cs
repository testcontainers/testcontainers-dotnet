namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerMessageBrokerConfiguration" />
  [PublicAPI]
  public class RabbitMqTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    private const string RabbitMqImage = "rabbitmq:3.7.28";

    private const int RabbitMqPort = 5672;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqTestcontainerConfiguration" /> class.
    /// </summary>
    public RabbitMqTestcontainerConfiguration()
      : this(RabbitMqImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public RabbitMqTestcontainerConfiguration(string image)
      : base(image, RabbitMqPort)
    {
    }

    /// <inheritdoc />
    public override string Username
    {
      get => this.Environments["RABBITMQ_DEFAULT_USER"];
      set => this.Environments["RABBITMQ_DEFAULT_USER"] = value;
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["RABBITMQ_DEFAULT_PASS"];
      set => this.Environments["RABBITMQ_DEFAULT_PASS"] = value;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
