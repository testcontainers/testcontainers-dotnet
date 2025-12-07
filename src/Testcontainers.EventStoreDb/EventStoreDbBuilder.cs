namespace Testcontainers.EventStoreDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
[Obsolete("Use KurrentDB instead of the EventStoreDB module. More info: https://www.kurrent.io/blog/kurrent-re-brand-faq.")]
public sealed class EventStoreDbBuilder : ContainerBuilder<EventStoreDbBuilder, EventStoreDbContainer, EventStoreDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string EventStoreDbImage = "eventstore/eventstore:22.10.1-buster-slim";

    public const ushort EventStoreDbPort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public EventStoreDbBuilder()
        : this(EventStoreDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>eventstore/eventstore:22.10.1-buster-slim</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/eventstore/eventstore/tags" />.
    /// </remarks>
    public EventStoreDbBuilder(string image)
        : this(new EventStoreDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/eventstore/eventstore/tags" />.
    /// </remarks>
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