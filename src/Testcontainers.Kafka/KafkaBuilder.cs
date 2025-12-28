namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KafkaBuilder : ContainerBuilder<KafkaBuilder, KafkaContainer, KafkaConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string KafkaImage = "confluentinc/cp-kafka:7.5.12";

    public const ushort KafkaPort = 9092;

    public const ushort BrokerPort = 9093;

    public const ushort ControllerPort = 9094;

    public const ushort ZooKeeperPort = 2181;

    public const string ClusterId = "4L6g3nShT-eMCtK--X86sw";

    public const string NodeId = "1";

    public const string StartupScriptFilePath = "/testcontainers.sh";

    private const string ProtocolPrefix = "TC";

    private static readonly IKafkaVendorConfiguration[] VendorConfigurations = new[] { ApacheConfiguration.Instance, ConfluentConfiguration.Instance };

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public KafkaBuilder()
        : this(KafkaImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>confluentinc/cp-kafka:7.5.12</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/confluentinc/cp-kafka/tags" />.
    /// </remarks>
    public KafkaBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/confluentinc/cp-kafka/tags" />.
    /// </remarks>
    public KafkaBuilder(IImage image)
        : this(new KafkaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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

    /// <summary>
    /// Adds a listener to the Kafka configuration in the format <c>host:port</c>.
    /// </summary>
    /// <remarks>
    ///
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
    ///     <item>
    ///         <description>
    ///             <c>PLAINTEXT://0.0.0.0:9092</c>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>BROKER://0.0.0.0:9093</c>
    ///             <p>If ZooKeeper is selected.</p>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>CONTROLLER://0.0.0.0:9094</c>
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="kafka">The Kafka connection string.</param>
    /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    public KafkaBuilder WithListener(string kafka)
    {
        var index = DockerResourceConfiguration.Listeners == null ? 0 : DockerResourceConfiguration.Listeners.Count();
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

        return Merge(DockerResourceConfiguration, new KafkaConfiguration(listeners: listeners, advertisedListeners: listeners))
            .WithEnvironment("KAFKA_LISTENERS", string.Join(",", updatedListeners))
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", string.Join(",", updatedListenersSecurityProtocolMap))
            .WithNetworkAliases(host);
    }

    /// <summary>
    /// Configures the Kafka container to use the specified consensus protocol.
    /// </summary>
    /// <param name="consensusProtocol">The consensus protocol to use.</param>
    /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    private KafkaBuilder WithConsensusProtocol(ConsensusProtocol consensusProtocol)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(consensusProtocol: consensusProtocol));
    }

    /// <summary>
    /// Configures the Kafka container to use the KRaft consensus protocol.
    /// </summary>
    /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    public KafkaBuilder WithKRaft()
    {
        return WithConsensusProtocol(ConsensusProtocol.KRaft)
            .WithEnvironment("KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER")
            .WithEnvironment("KAFKA_PROCESS_ROLES", "broker,controller")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Transitioning from RECOVERY to RUNNING.*"));
    }

    /// <summary>
    /// Configures the Kafka container to use the ZooKeeper consensus protocol.
    /// </summary>
    /// <param name="connectionString">The external ZooKeeper connection string. If <c>null</c>, vendor-specific defaults will be used.</param>
    /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    public KafkaBuilder WithZooKeeper(string connectionString = null)
    {
        return WithConsensusProtocol(ConsensusProtocol.ZooKeeper)
            .WithPortBinding(ZooKeeperPort, connectionString == null)
            .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", connectionString ?? $"localhost:{ZooKeeperPort}")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("\\[KafkaServer id=\\d+\\] started"));
    }

    /// <summary>
    /// Explicitly sets the Kafka vendor for the builder configuration. Use this method
    /// only when the vendor cannot be automatically detected from the image. This
    /// ensures the container is configured correctly since different vendors may
    /// require different configurations.
    /// </summary>
    /// <remarks>
    /// This method is typically necessary when using a custom Kafka image built on top
    /// of an existing base image, such as those from Apache or Confluent. By specifying
    /// the vendor explicitly, the appropriate configuration for the Kafka container is
    /// applied.
    ///
    /// <para>
    /// Vendor detection is normally automatic based on the image name. However, if the
    /// image name does not clearly indicate the vendor, this method allows you to specify
    /// it manually.
    /// </para>
    ///
    /// </remarks>
    /// <param name="vendor">The Kafka vendor to use for configuration.</param>
    /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    public KafkaBuilder WithVendor(KafkaVendor vendor)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(vendor: vendor));
    }

    /// <inheritdoc />
    public override KafkaContainer Build()
    {
        Validate();

        KafkaBuilder kafkaBuilder;

        // Instead of this approach, should we consider using a builder for each vendor?
        var vendorConfiguration = VendorConfigurations.Single(v => v.Vendor.Equals(DockerResourceConfiguration.Vendor) || v.IsImageFromVendor(DockerResourceConfiguration.Image));

        // If the user hasn't set a consensus protocol, use the vendor's default configuration.
        if (DockerResourceConfiguration.ConsensusProtocol.HasValue)
        {
            kafkaBuilder = this;
        }
        else if (vendorConfiguration.ConsensusProtocol == ConsensusProtocol.KRaft)
        {
            kafkaBuilder = WithKRaft();
        }
        else if (vendorConfiguration.ConsensusProtocol == ConsensusProtocol.ZooKeeper)
        {
            kafkaBuilder = WithZooKeeper();
        }
        else
        {
            throw new ArgumentException($"No default configuration available for vendor '{vendorConfiguration.Vendor}'.");
        }

        // Validate that the configuration is compatible with the vendor's image.
        vendorConfiguration.Validate(DockerResourceConfiguration);

        var startupKafkaBuilder = kafkaBuilder.WithStartupCallback((container, ct) =>
        {
            var startupScript = vendorConfiguration.CreateStartupScript(kafkaBuilder.DockerResourceConfiguration, container);
            return container.CopyAsync(Encoding.Default.GetBytes(startupScript), StartupScriptFilePath, fileMode: Unix.FileMode755, ct: ct);
        });

        return new KafkaContainer(startupKafkaBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override KafkaBuilder Init()
    {
        return base.Init()
            .WithPortBinding(KafkaPort, true)
            .WithPortBinding(BrokerPort, true)
            .WithEnvironment("KAFKA_LISTENERS", $"PLAINTEXT://:{KafkaPort},BROKER://:{BrokerPort},CONTROLLER://:{ControllerPort}")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
            .WithEnvironment("KAFKA_BROKER_ID", "1")
            .WithEnvironment("KAFKA_NODE_ID", NodeId)
            .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", $"{NodeId}@localhost:{ControllerPort}")
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
    protected override void Validate()
    {
        const string message = "Vendor was not set or could not be extracted from the image.";

        base.Validate();

        Predicate<KafkaVendor?> vendorNotFound = value => value == null && !VendorConfigurations.Any(v => v.IsImageFromVendor(DockerResourceConfiguration.Image));

        _ = Guard.Argument(DockerResourceConfiguration.Vendor, nameof(DockerResourceConfiguration.Vendor))
            .ThrowIf(argument => vendorNotFound(argument.Value), argument => new ArgumentException(message, argument.Name));
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