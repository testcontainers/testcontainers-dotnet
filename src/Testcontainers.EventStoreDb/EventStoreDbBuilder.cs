using DotNet.Testcontainers.Images;

namespace Testcontainers.EventStoreDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
[Obsolete("Use KurrentDB instead of the EventStoreDB module. More info: https://www.kurrent.io/blog/kurrent-re-brand-faq.")]
public sealed class EventStoreDbBuilder : ContainerBuilder<EventStoreDbBuilder, EventStoreDbContainer, EventStoreDbConfiguration>
{
    public const string EventStoreDbImage = "eventstore/eventstore:22.10.1-buster-slim";

    public const ushort EventStoreDbPort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public EventStoreDbBuilder()
        : this(new EventStoreDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(EventStoreDbImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/eventstore/eventstore/tags">https://hub.docker.com/r/eventstore/eventstore/tags</see>.</param>
    public EventStoreDbBuilder(string image)
        : this(new EventStoreDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public EventStoreDbBuilder(IImage image)
        : this(new EventStoreDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private EventStoreDbBuilder(EventStoreDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override EventStoreDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override EventStoreDbContainer Build()
    {
        return new EventStoreDbContainer(DockerResourceConfiguration);
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
    protected override EventStoreDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new EventStoreDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override EventStoreDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new EventStoreDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override EventStoreDbBuilder Merge(EventStoreDbConfiguration oldValue, EventStoreDbConfiguration newValue)
    {
        return new EventStoreDbBuilder(new EventStoreDbConfiguration(oldValue, newValue));
    }
}