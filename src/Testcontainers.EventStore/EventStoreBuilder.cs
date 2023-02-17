namespace Testcontainers.EventStore
{
    public class EventStoreBuilder : ContainerBuilder<EventStoreBuilder, EventStoreContainer, EventStoreConfiguration>
    {
        private const string EventStoreImage = "eventstore/eventstore:21.2.0-buster-slim";

        public const ushort EventStorePort = 2113;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBuilder" /> class.
        /// </summary>
        public EventStoreBuilder()
            : this(new EventStoreConfiguration())
        {
            DockerResourceConfiguration = Init().DockerResourceConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBuilder" /> class.
        /// </summary>
        /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
        private EventStoreBuilder(EventStoreConfiguration dockerResourceConfiguration)
            : base(dockerResourceConfiguration)
        {
            DockerResourceConfiguration = dockerResourceConfiguration;
        }

        /// <inheritdoc />
        protected override EventStoreConfiguration DockerResourceConfiguration { get; }


        /// <inheritdoc />
        public override EventStoreContainer Build()
        {
            return new EventStoreContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
        }

        /// <inheritdoc />
        protected override EventStoreBuilder Init()
        {
            return base.Init()
                .WithImage(EventStoreImage)
                .WithPortBinding(EventStorePort, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
                .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
                .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
                .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
                .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
                .WithEnvironment("EVENTSTORE_INSECURE", "true");
        }

        /// <inheritdoc />
        protected override EventStoreBuilder Clone(IContainerConfiguration resourceConfiguration)
        {
            return Merge(DockerResourceConfiguration, new EventStoreConfiguration(resourceConfiguration));
        }

        /// <inheritdoc />
        protected override EventStoreBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        {
            return Merge(DockerResourceConfiguration, new EventStoreConfiguration(resourceConfiguration));
        }

        /// <inheritdoc />
        protected override EventStoreBuilder Merge(EventStoreConfiguration oldValue, EventStoreConfiguration newValue)
        {
            return new EventStoreBuilder(new EventStoreConfiguration(oldValue, newValue));
        }
    }
}