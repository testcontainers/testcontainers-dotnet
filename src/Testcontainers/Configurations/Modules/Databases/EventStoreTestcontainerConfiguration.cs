namespace DotNet.Testcontainers.Configurations
{
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
    }
  }
}
