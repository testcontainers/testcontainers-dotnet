namespace Testcontainers.EventStoreDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class EventStoreDbBuilder : ContainerBuilder<EventStoreDbBuilder, EventStoreDbContainer, EventStoreDbConfiguration>
{
    public const string EventStoreDbImage = "eventstore/eventstore:22.10.1-buster-slim";

    public const ushort EventStoreDbPort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    public EventStoreDbBuilder()
        : this(new EventStoreDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private EventStoreDbBuilder(EventStoreDbConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override EventStoreDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override EventStoreDbContainer Build()
    {
        return new EventStoreDbContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override EventStoreDbBuilder Init()
    {
        return base.Init()
            .WithImage(EventStoreDbImage)
            .WithPortBinding(EventStoreDbPort, true)
            .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
            .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
            .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("EVENTSTORE_INSECURE", "true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
    }

    /// <inheritdoc />
    protected override EventStoreDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new EventStoreDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override EventStoreDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new EventStoreDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override EventStoreDbBuilder Merge(EventStoreDbConfiguration oldValue, EventStoreDbConfiguration newValue)
    {
        return new EventStoreDbBuilder(new EventStoreDbConfiguration(oldValue, newValue));
    }
}