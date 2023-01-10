namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class EventStoreTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string EventStoreImageImage = "eventstore/eventstore:21.2.0-buster-slim";

    private const int EventStorePort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreTestcontainerConfiguration" /> class.
    /// </summary>
    public EventStoreTestcontainerConfiguration()
      : this(EventStoreImageImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public EventStoreTestcontainerConfiguration(string image)
      : base(image, EventStorePort)
    {
      this.Environments["EVENTSTORE_CLUSTER_SIZE"] = "1";
      this.Environments["EVENTSTORE_RUN_PROJECTIONS"] = "All";
      this.Environments["EVENTSTORE_START_STANDARD_PROJECTIONS"] = "true";
      this.Environments["EVENTSTORE_EXT_TCP_PORT"] = "1113";
      this.Environments["EVENTSTORE_EXT_HTTP_PORT"] = "2113";
      this.Environments["EVENTSTORE_INSECURE"] = "true";
      this.Environments["EVENTSTORE_ENABLE_EXTERNAL_TCP"] = "true";
      this.Environments["EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP"] = "true";
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
