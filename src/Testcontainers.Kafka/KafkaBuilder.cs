using Testcontainers.Kafka.Vendors;

namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KafkaBuilder : ContainerBuilder<KafkaBuilder, KafkaContainer, KafkaConfiguration>
{
    public const string KafkaImage = "confluentinc/cp-kafka:7.5.1";

    public const string KafkaNodeId = "1";

    public const ushort KafkaPort = 9092;

    public const ushort BrokerPort = 9093;

    public const ushort ControllerPort = 9094;

    public const ushort ZookeeperPort = 2181;

    public const string ClusterId = "4L6g3nShT-eMCtK--X86sw";

    public const string StartupScriptFilePath = "/testcontainers.sh";

    private const string ProtocolPrefix = "TC";

    private static readonly IKafkaVendor[] Vendors =
    [
        new ConfluentVendor(),
        new ApacheKafkaVendor(),
    ];

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

        var kafkaVendor = GetKafkaVendor();

        var kafkaBuilder = new KafkaBuilder(DockerResourceConfiguration);
        if (!DockerResourceConfiguration.ConsensusProtocol.HasValue)
        {
            kafkaBuilder = kafkaBuilder
                .WithConsensusProtocol(kafkaVendor.DefaultConsensusProtocol);
        }

        kafkaVendor.ValidateConfigurationAndThrow(kafkaBuilder.DockerResourceConfiguration);

        kafkaBuilder = kafkaBuilder.DockerResourceConfiguration.ConsensusProtocol switch
        {
            KafkaConsensusProtocol.KRaft => kafkaBuilder.WithKRaftSupport(kafkaVendor),
            KafkaConsensusProtocol.Zookeeper => kafkaBuilder.WithZookeeperSupport(kafkaVendor),
            _ => throw new ArgumentOutOfRangeException(nameof(DockerResourceConfiguration.ConsensusProtocol)),
        };
        return new KafkaContainer(kafkaBuilder.DockerResourceConfiguration);
    }

    /// <summary>
    /// Adds a listener to the Kafka configuration in the format <c>host:port</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The host will be included as a network alias, allowing additional connections
    /// to the Kafka broker within the same container network.
    /// </para>
    /// 
    /// <para>
    /// This method is useful for registering custom listeners beyond the default ones,
    /// enabling specific connection points for Kafka brokers.
    /// </para>
    /// 
    /// <para>
    /// Default listeners include:
    /// </para>
    /// 
    /// <list type="bullet">
    /// <item>
    /// <description><c>PLAINTEXT://0.0.0.0:9092</c></description>
    /// </item>
    /// <item>
    /// <description><c>BROKER://0.0.0.0:9093</c> (if Zookeeper is used)</description>
    /// </item>
    /// <item>
    /// <description><c>CONTROLLER://0.0.0.0:9094</c></description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="kafka">Kafka connection string</param>
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

    /// <summary>
    /// Configures the Kafka to use the KRaft consensus protocol instead of Zookeeper.
    /// </summary>
    /// <returns>An updated instance of the <see cref="KafkaBuilder"/> configured with KRaft.</returns>
    public KafkaBuilder WithKRaft()
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(
            consensusProtocol: KafkaConsensusProtocol.KRaft));
    }

    /// <summary>
    /// Configures the Kafka to use Zookeeper as the consensus protocol.<br/>
    /// If no external Zookeeper connection string is provided, a default local Zookeeper instance will be set up within the container
    /// if supported.
    /// </summary>
    /// <param name="connectionString">The optional external Zookeeper connection string. If <c>null</c>, a default local setup will be used.</param>
    /// <returns>An updated instance of the <see cref="KafkaBuilder"/> configured with Zookeeper.</returns>
    public KafkaBuilder WithZookeeper(string? connectionString = null)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(
            consensusProtocol: KafkaConsensusProtocol.Zookeeper,
            externalZookeeperConnectionString: connectionString));
    }

    private KafkaBuilder WithConsensusProtocol(KafkaConsensusProtocol consensusProtocol)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(
            consensusProtocol: consensusProtocol));
    }

    /// <summary>
    /// Configures the Kafka container with the specified cluster ID.
    /// </summary>
    /// <param name="clusterId">The unique identifier for the Kafka cluster.</param>
    /// <returns>The current <see cref="KafkaBuilder"/> instance configured with the specified cluster ID.</returns>
    public KafkaBuilder WithClusterId(string clusterId)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration())
            .WithEnvironment("CLUSTER_ID", clusterId);
    }

    /// <summary>
    /// Explicitly sets the Kafka Docker image vendor. Use this method only when the image vendor cannot be automatically detected
    /// from the provided image name. This allows the container to be set up properly since different vendors have different base configurations.
    /// </summary>
    /// <remarks>
    /// This method is typically required when using a custom Kafka image that is built on top of an existing base image,
    /// such as a Confluent or Apache Kafka base image. By specifying the vendor, the appropriate configurations
    /// for the Kafka container can be applied.
    /// <para>
    /// Automatic detection of the vendor is based on the image name. However, if the image name does not clearly
    /// indicate the vendor, this method allows you to manually specify the correct one.
    /// </para>
    /// </remarks>
    /// <param name="imageVendor">The Kafka image vendor to use for configuration.</param>
    /// <returns>
    /// A configured instance of the <see cref="KafkaBuilder"/> class with the supplied image vendor settings.
    /// </returns>
    public KafkaBuilder WithImageVendor(KafkaImageVendor imageVendor)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(imageVendor: imageVendor));
    }

    private IKafkaVendor GetKafkaVendor()
    {
        if (!DockerResourceConfiguration.ImageVendor.HasValue)
        {
            var detectedVendor = Vendors.FirstOrDefault(v => v.ImageBelongsToVendor(DockerResourceConfiguration.Image));
            if (detectedVendor is not null)
            {
                return detectedVendor;
            }

            // Using Confluent one for backward compatibility
            return Vendors.Single(x => x.ImageVendor == KafkaImageVendor.Confluent);
        }

        return Vendors.Single(x => x.ImageVendor == DockerResourceConfiguration.ImageVendor);
    }

    private KafkaBuilder WithKRaftSupport(IKafkaVendor kafkaVendor)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration())
            .WithEnvironment("KAFKA_PROCESS_ROLES", "broker,controller")
            .WithEnvironment("KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Transitioning from RECOVERY to RUNNING.*"))
            .WithStartupCallback((container, ct) =>
            {
                var startupScript = kafkaVendor.GetStartupScript(new StartupScriptContext
                {
                    Container = container,
                    Configuration = DockerResourceConfiguration,
                });
                return container.CopyAsync(Encoding.Default.GetBytes(startupScript), StartupScriptFilePath, Unix.FileMode755, ct);
            });
    }

    private KafkaBuilder WithZookeeperSupport(IKafkaVendor kafkaVendor)
    {
        var kafkaBuilder = Merge(DockerResourceConfiguration, new KafkaConfiguration())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(@"\[KafkaServer id=\d+\] started"))
            .WithStartupCallback((container, ct) =>
            {
                var startupScript = kafkaVendor.GetStartupScript(new StartupScriptContext
                {
                    Container = container,
                    Configuration = DockerResourceConfiguration,
                });
                return container.CopyAsync(Encoding.Default.GetBytes(startupScript), StartupScriptFilePath, Unix.FileMode755, ct);
            });

        if (DockerResourceConfiguration.ExternalZookeeperConnectionString is null)
        {
            return kafkaBuilder
                .WithPortBinding(ZookeeperPort, true)
                .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", $"localhost:{ZookeeperPort}");
        }

        // External Zookeeper instance is provided. There is no need to expose Zookeeper port ourselves.
        return kafkaBuilder
            .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", DockerResourceConfiguration.ExternalZookeeperConnectionString);
    }

    /// <inheritdoc />
    protected override KafkaBuilder Init()
    {
        return base.Init()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaPort, true)
            .WithPortBinding(BrokerPort, true)
            .WithEnvironment("KAFKA_LISTENERS", $"PLAINTEXT://:{KafkaPort},BROKER://:{BrokerPort},CONTROLLER://:{ControllerPort}")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
            .WithEnvironment("KAFKA_BROKER_ID", "1")
            .WithEnvironment("KAFKA_NODE_ID", KafkaNodeId)
            .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", $"{KafkaNodeId}@localhost:{ControllerPort}")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_NUM_PARTITIONS", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_MIN_ISR", "1")
            .WithEnvironment("KAFKA_LOG_FLUSH_INTERVAL_MESSAGES", long.MaxValue.ToString())
            .WithEnvironment("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0")
            .WithEnvironment("CLUSTER_ID", ClusterId)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand($"while [ ! -f {StartupScriptFilePath} ]; do sleep 0.1; done; {StartupScriptFilePath}");
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