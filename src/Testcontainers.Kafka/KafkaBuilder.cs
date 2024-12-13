using System.Collections.Generic;
using System.Linq;

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
    /// Add a listener in the format host:port.
    /// Host will be included as a network alias.
    /// Use it to register additional connections to the Kafka within the same container network.
    ///
    /// Default listeners: PLAINTEXT://0.0.0.0:9092, BROKER://0.0.0.0:9093, CONTROLLER://0.0.0.0:9094
    /// </summary>
    /// <param name="kafka"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public KafkaBuilder WithListener(string kafka)
    {
        var host = kafka.Split(':')[0];

        var index = (DockerResourceConfiguration.Listeners ?? new List<string>()).Count();
        var protocol = $"{ProtocolPrefix}-{index}";
        var listener = $"{protocol}://{kafka}";
        var listenerSecurityProtocolMap = $"{protocol}:PLAINTEXT";
        
        var currentListeners = this.DockerResourceConfiguration.Environments["KAFKA_LISTENERS"];
        var currentListenersSecurityProtocolMap = this.DockerResourceConfiguration.Environments["KAFKA_LISTENER_SECURITY_PROTOCOL_MAP"];

        return this.Merge(DockerResourceConfiguration, new KafkaConfiguration(listeners:new List<string>{ listener }, advertisedListeners: new List<string>{ listener }))
            .WithEnvironment(new Dictionary<string, string>
            {
                { "KAFKA_LISTENERS", $"{currentListeners},{string.Join(",", listener)}" },
                { "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", $"{currentListenersSecurityProtocolMap},{string.Join(",", listenerSecurityProtocolMap)}" }
            })
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
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT")
            .WithEnvironment("KAFKA_NODE_ID", "1")
            .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", "1@localhost:" + ControllerPort)
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
                var additionalAdvertisedListeners = 
                    (container.AdvertisedListeners != null && container.AdvertisedListeners.Any()) ? "," + string.Join(",", container.AdvertisedListeners) : "";
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
                startupScript.Append("export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://" + container.Hostname + ":" + container.GetMappedPublicPort(KafkaPort) + ",BROKER://" + container.IpAddress + ":" + BrokerPort + additionalAdvertisedListeners);
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