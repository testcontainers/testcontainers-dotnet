using Testcontainers.Kafka;

namespace Testcontainers.ApacheKafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ApacheKafkaBuilder : ContainerBuilder<ApacheKafkaBuilder, KafkaContainer, KafkaConfiguration>
{
    public const ushort KafkaPort = 9092;
    public const ushort BrokerPort = 9093;
    public const ushort ControllerPort = 9094;
    
    private const string KafkaImage = "apache/kafka:3.9.0";
    private const string StarterScript = "/testcontainers.sh";
    private const string KafkaNodeId = "1";

    /// <summary>
    /// Initializes a new instance of the <see cref="ApacheKafkaBuilder" /> class.
    /// </summary>
    public ApacheKafkaBuilder() : this(new KafkaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApacheKafkaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ApacheKafkaBuilder(KafkaConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    public override KafkaContainer Build()
    {
        Validate();
        return new KafkaContainer(DockerResourceConfiguration);
    }
    
    /// <inheritdoc />
    protected override KafkaConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override ApacheKafkaBuilder Init()
    {
        return base.Init()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaPort, true)
            .WithEnvironment("KAFKA_LISTENERS", $"PLAINTEXT://:{KafkaPort},BROKER://:{BrokerPort},CONTROLLER://:{ControllerPort}")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
            .WithEnvironment("KAFKA_PROCESS_ROLES", "broker,controller")
            .WithEnvironment("KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER")
            .WithEnvironment("KAFKA_NODE_ID", KafkaNodeId)
            .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", $"{KafkaNodeId}@localhost:{ControllerPort}")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_NUM_PARTITIONS", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_MIN_ISR", "1")
            .WithEnvironment("KAFKA_LOG_FLUSH_INTERVAL_MESSAGES", long.MaxValue.ToString())
            .WithEnvironment("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0")
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand($"while [ ! -f {StarterScript} ]; do sleep 0.1; done; {StarterScript}")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Transitioning from RECOVERY to RUNNING.*"))
            .WithStartupCallback((container, ct) =>
            {
                const char lf = '\n';
                var startupScript = new StringBuilder();
                startupScript.Append("#!/bin/bash");
                startupScript.Append(lf);

                var brokerListener = $"BROKER://{container.IpAddress}:{BrokerPort}";
                var listener = $"PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaPort)}";
                startupScript.Append($"export KAFKA_ADVERTISED_LISTENERS={listener},{brokerListener}");
                startupScript.Append(lf);
                startupScript.Append("exec /etc/kafka/docker/run");
                return container.CopyAsync(Encoding.Default.GetBytes(startupScript.ToString()), StarterScript, Unix.FileMode755, ct);
            });
    }

    /// <inheritdoc />
    protected override ApacheKafkaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ApacheKafkaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ApacheKafkaBuilder Merge(KafkaConfiguration oldValue, KafkaConfiguration newValue)
    {
        return new ApacheKafkaBuilder(new KafkaConfiguration(oldValue, newValue));
    }
}