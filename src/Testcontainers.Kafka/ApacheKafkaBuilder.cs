using Testcontainers.Kafka;

namespace Testcontainers.ApacheKafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ApacheKafkaBuilder : BaseKafkaBuilder<ApacheKafkaBuilder>
{
    public override string KafkaImage => "apache/kafka:3.9.0";
    protected override string ReadyMessage => ".*Transitioning from RECOVERY to RUNNING.*";

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
    protected override KafkaConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override ApacheKafkaBuilder Init()
    {
        return base.Init()
            .WithEnvironment("KAFKA_PROCESS_ROLES", "broker,controller")
            .WithEnvironment("KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER");
    }

    protected override string GetStartupScript(KafkaContainer container)
    {
        var additionalAdvertisedListeners = string.Join(",", container.AdvertisedListeners ?? Array.Empty<string>());
        var kafkaListener = $"PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaConfiguration.KafkaPort)}";
        var brokerListener = $"BROKER://{container.IpAddress}:{KafkaConfiguration.BrokerPort}";
        return $"""
                #!/bin/bash
                export KAFKA_ADVERTISED_LISTENERS={kafkaListener},{brokerListener},{additionalAdvertisedListeners}
                echo '' > /etc/kafka/docker/ensure
                exec /etc/kafka/docker/run
                """;
    }
    
    /// <inheritdoc />
    protected override ApacheKafkaBuilder Merge(KafkaConfiguration oldValue, KafkaConfiguration newValue)
    {
        return new ApacheKafkaBuilder(new KafkaConfiguration(oldValue, newValue));
    }
}