namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KafkaBuilder : ContainerBuilder<KafkaBuilder, KafkaContainer, KafkaConfiguration>
{
    public const string KafkaImage = "confluentinc/cp-kafka:6.1.9";

    public const ushort KafkaPort = 9092;

    public const ushort BrokerPort = 9093;

    public const ushort ZookeeperPort = 2181;

    public const string StartupScriptFilePath = "/testcontainers.sh";

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
        return new KafkaContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override KafkaBuilder Init()
    {
        return base.Init()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaPort, true)
            .WithPortBinding(BrokerPort, true)
            .WithPortBinding(ZookeeperPort, true)
            .WithEnvironment("KAFKA_LISTENERS", "PLAINTEXT://0.0.0.0:" + KafkaPort + ",BROKER://0.0.0.0:" + BrokerPort)
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,PLAINTEXT:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
            .WithEnvironment("KAFKA_BROKER_ID", "1")
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
                startupScript.Append("export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://" + container.Hostname + ":" + container.GetMappedPublicPort(KafkaPort) + ",BROKER://" + container.Hostname + ":" + BrokerPort);
                startupScript.Append(lf);
                startupScript.Append("echo '' > /etc/confluent/docker/ensure");
                startupScript.Append(lf);
                startupScript.Append("/etc/confluent/docker/run");
                return container.CopyFileAsync(StartupScriptFilePath, Encoding.Default.GetBytes(startupScript.ToString()), 493, ct: ct);
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