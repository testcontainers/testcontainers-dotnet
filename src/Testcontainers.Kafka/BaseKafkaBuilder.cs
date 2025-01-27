namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public abstract class BaseKafkaBuilder<TBuilderEntity> : ContainerBuilder<TBuilderEntity, KafkaContainer, KafkaConfiguration>
    where TBuilderEntity : BaseKafkaBuilder<TBuilderEntity>
{
    public abstract string KafkaImage { get; }
    public const string StartupScriptPath = "/testcontainers.sh";

    protected abstract string ReadyMessage { get; }

    protected abstract string GetStartupScript(KafkaContainer container);

    private const string ProtocolPrefix = "TC";

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseKafkaBuilder{TBuilderEntity}" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    protected BaseKafkaBuilder(KafkaConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
    }

    /// <inheritdoc />
    public override KafkaContainer Build()
    {
        Validate();
        return new KafkaContainer(DockerResourceConfiguration);
    }

    /// <summary>
    /// Adds a listener to the Kafka configuration in the format <c>host:port</c>.
    /// </summary>
    /// <remarks>
    /// The host will be included as a network alias, allowing additional connections
    /// to the Kafka broker within the same container network.
    ///
    /// This method is useful for registering custom listeners beyond the default ones,
    /// enabling specific connection points for Kafka brokers.
    ///
    /// Default listeners include:
    /// - <c>PLAINTEXT://:9092</c>
    /// - <c>BROKER://:9093</c>
    /// - <c>CONTROLLER://:9094</c>
    /// </remarks>
    /// <param name="kafka">The MsSql database.</param>
    /// <returns>A configured instance of <see cref="BaseKafkaBuilder{TBuilderEntity}" />.</returns>
    public TBuilderEntity WithListener(string kafka)
    {
        var index = DockerResourceConfiguration.Listeners?.Count() ?? 0;
        var protocol = $"{ProtocolPrefix}-{index}";
        var listener = $"{protocol}://{kafka}";
        var listenerSecurityProtocolMap = $"{protocol}:PLAINTEXT";

        var listeners = new[] { listener };
        var listenersSecurityProtocolMap = new[] { listenerSecurityProtocolMap };

        var host = kafka.Split(':')[0];

        var updatedListeners = DockerResourceConfiguration.Environments["KAFKA_LISTENERS"]
            .Split(',')
            .Concat(listeners);

        var updatedListenersSecurityProtocolMap = DockerResourceConfiguration.Environments["KAFKA_LISTENER_SECURITY_PROTOCOL_MAP"]
            .Split(',')
            .Concat(listenersSecurityProtocolMap);

        return Merge(DockerResourceConfiguration, new KafkaConfiguration(listeners, listeners))
            .WithEnvironment("KAFKA_LISTENERS", string.Join(",", updatedListeners))
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", string.Join(",", updatedListenersSecurityProtocolMap))
            .WithNetworkAliases(host);
    }

    /// <inheritdoc />
    protected override TBuilderEntity Init()
    {
        return base.Init()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaConfiguration.KafkaPort, true)
            .WithPortBinding(KafkaConfiguration.BrokerPort, true)
            .WithEnvironment("KAFKA_LISTENERS",
                $"PLAINTEXT://:{KafkaConfiguration.KafkaPort},BROKER://:{KafkaConfiguration.BrokerPort},CONTROLLER://:{KafkaConfiguration.ControllerPort}")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
            .WithEnvironment("KAFKA_BROKER_ID", "1")
            .WithEnvironment("KAFKA_NODE_ID", "1")
            .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", "1@localhost:" + KafkaConfiguration.ControllerPort)
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_NUM_PARTITIONS", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_MIN_ISR", "1")
            .WithEnvironment("KAFKA_LOG_FLUSH_INTERVAL_MESSAGES", long.MaxValue.ToString())
            .WithEnvironment("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0")
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("while [ ! -f " + StartupScriptPath + " ]; do sleep 0.1; done; " + StartupScriptPath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(ReadyMessage))
            .WithStartupCallback((container, ct) =>
                container.CopyAsync(Encoding.Default.GetBytes(GetStartupScript(container)), StartupScriptPath, Unix.FileMode755, ct));
    }

    /// <inheritdoc />
    protected override TBuilderEntity Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TBuilderEntity Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }
}