namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KafkaBuilder : ContainerBuilder<KafkaBuilder, KafkaContainer, KafkaConfiguration>
{
    public const string KafkaImage = "confluentinc/cp-kafka:6.1.9";

    public const ushort KafkaPort = 9092;

    public const ushort BrokerPort = 9093;

    public const ushort ControllerPort = 9094;

    public const ushort ZookeeperPort = 2181;

    public const string StartupScriptFilePath = "/testcontainers.sh";

    private const string ProtocolPrefix = "TC";

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    public KafkaBuilder()
        : this(new KafkaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KafkaBuilder(KafkaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override KafkaConfiguration DockerResourceConfiguration { get; }

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
    /// - <c>PLAINTEXT://0.0.0.0:9092</c>
    /// - <c>BROKER://0.0.0.0:9093</c>
    /// - <c>CONTROLLER://0.0.0.0:9094</c>
    /// </remarks>
    /// <param name="kafka">The MsSql database.</param>
    /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    public KafkaBuilder WithListener(string kafka)
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
    protected override KafkaBuilder Init()
    {
        return base.Init()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaPort, true)
            .WithPortBinding(BrokerPort, true)
            .WithPortBinding(ZookeeperPort, true)
            .WithEnvironment("KAFKA_LISTENERS", $"PLAINTEXT://0.0.0.0:{KafkaPort},BROKER://0.0.0.0:{BrokerPort},CONTROLLER://0.0.0.0:{ControllerPort}")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
            .WithEnvironment("KAFKA_BROKER_ID", "1")
            .WithEnvironment("KAFKA_NODE_ID", "1")
            .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", "1@localhost:" + ControllerPort)
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_NUM_PARTITIONS", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_MIN_ISR", "1")
            .WithEnvironment("KAFKA_LOG_FLUSH_INTERVAL_MESSAGES", long.MaxValue.ToString())
            .WithEnvironment("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0")
            .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "localhost:" + ZookeeperPort)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("while [ ! -f " + StartupScriptFilePath + " ]; do sleep 0.1; done; " + StartupScriptFilePath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("\\[KafkaServer id=\\d+\\] started"))
            .WithStartupCallback((container, ct) =>
            {
                const char lf = '\n';
                var additionalAdvertisedListeners = string.Join(",", container.AdvertisedListeners ?? Array.Empty<string>());
                var startupScript = new StringBuilder();
                startupScript.Append("#!/bin/bash");
                startupScript.Append(lf);
                startupScript.Append("echo 'clientPort=" + ZookeeperPort + "' > zookeeper.properties");
                startupScript.Append(lf);
                startupScript.Append("echo 'dataDir=/var/lib/zookeeper/data' >> zookeeper.properties");
                startupScript.Append(lf);
                startupScript.Append("echo 'dataLogDir=/var/lib/zookeeper/log' >> zookeeper.properties");
                startupScript.Append(lf);
                startupScript.Append("zookeeper-server-start zookeeper.properties &");
                startupScript.Append(lf);
                startupScript.Append("export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://" + container.Hostname + ":" + container.GetMappedPublicPort(KafkaPort) + ",BROKER://" + container.IpAddress + ":" + BrokerPort + "," + additionalAdvertisedListeners);
                startupScript.Append(lf);
                startupScript.Append("echo '' > /etc/confluent/docker/ensure");
                startupScript.Append(lf);
                startupScript.Append("exec /etc/confluent/docker/run");
                return container.CopyAsync(Encoding.Default.GetBytes(startupScript.ToString()), StartupScriptFilePath, Unix.FileMode755, ct);
            });
    }

    /// <inheritdoc />
    protected override KafkaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KafkaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KafkaBuilder Merge(KafkaConfiguration oldValue, KafkaConfiguration newValue)
    {
        return new KafkaBuilder(new KafkaConfiguration(oldValue, newValue));
    }
}