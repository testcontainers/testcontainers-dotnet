namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KafkaBuilder : BaseKafkaBuilder<KafkaBuilder>
{
    [Obsolete("This constant will be removed. Please use KafkaConfiguration.BrokerPort instead")]
    public const ushort BrokerPort = 9093;

    [Obsolete("This constant will be removed. Please use KafkaConfiguration.ControllerPort instead")]
    public const ushort ControllerPort = 9094;

    [Obsolete("This constant will be removed. Please use BaseKafkaBuilder.StartupScriptPath instead")]
    public const string StartupScriptFilePath = "/testcontainers.sh";
    
    public const ushort ZookeeperPort = 2181;
    public override string KafkaImage => "confluentinc/cp-kafka:6.1.9";

    protected override string ReadyMessage => @"\[KafkaServer id=\d+\] started";

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    public KafkaBuilder() : this(new KafkaConfiguration())
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
    protected override KafkaBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ZookeeperPort, true)
            .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "localhost:" + ZookeeperPort);
    }

    protected override string GetStartupScript(KafkaContainer container)
    {
        var additionalAdvertisedListeners = string.Join(",", container.AdvertisedListeners ?? Array.Empty<string>());
        var kafkaListener = $"PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaConfiguration.KafkaPort)}";
        var brokerListener = $"BROKER://{container.IpAddress}:{KafkaConfiguration.BrokerPort}";
        return $"""
                #!/bin/bash
                echo 'clientPort={ZookeeperPort}' > zookeeper.properties
                echo 'dataDir=/var/lib/zookeeper/data' >> zookeeper.properties
                echo 'dataLogDir=/var/lib/zookeeper/log' >> zookeeper.properties
                zookeeper-server-start zookeeper.properties &
                export KAFKA_ADVERTISED_LISTENERS={kafkaListener},{brokerListener},{additionalAdvertisedListeners}
                echo '' > /etc/confluent/docker/ensure
                exec /etc/confluent/docker/run
                """;
    }
    
    /// <inheritdoc />
    protected override KafkaBuilder Merge(KafkaConfiguration oldValue, KafkaConfiguration newValue)
    {
        return new KafkaBuilder(new KafkaConfiguration(oldValue, newValue));
    }
}