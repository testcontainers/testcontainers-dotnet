namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerMessageBrokerConfiguration" />
  [PublicAPI]
  public class LocalStackTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    private const string LocalStackImage = "localstack/localstack:latest";

    private const int LocalStackPort = 4566;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackTestcontainerConfiguration" /> class.
    /// </summary>
    public LocalStackTestcontainerConfiguration()
      : this(LocalStackImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public LocalStackTestcontainerConfiguration(string image)
      : base(image, LocalStackPort)
    {
        this.Environments.Add("EXTERNAL_SERVICE_PORTS_START", "4510");
        this.Environments.Add("EXTERNAL_SERVICE_PORTS_END", "4559");
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(LocalStackPort);
  }
}
