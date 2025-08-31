namespace Testcontainers.Kafka;

/// <inheritdoc cref="IKafkaVendorConfiguration" />
internal sealed class ApacheConfiguration : IKafkaVendorConfiguration
{
    static ApacheConfiguration() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApacheConfiguration" /> class.
    /// </summary>
    private ApacheConfiguration() { }

    /// <summary>
    /// Gets the singleton instance of the Apache vendor configuration.
    /// </summary>
    public static IKafkaVendorConfiguration Instance { get; } = new ApacheConfiguration();

    /// <inheritdoc />
    public KafkaVendor Vendor => KafkaVendor.ApacheSoftwareFoundation;

    /// <inheritdoc />
    public ConsensusProtocol ConsensusProtocol => ConsensusProtocol.KRaft;

    /// <inheritdoc />
    public bool IsImageFromVendor(IImage image)
    {
        return image.Repository.Contains("apache") || image.Repository.Contains("bitnami");
    }

    /// <inheritdoc />
    public void Validate(KafkaConfiguration resourceConfiguration)
    {
        const string message =
            "Local ZooKeeper is not supported for Apache Kafka images. Configure an external ZooKeeper.";

        var isZooKeeperConsensus =
            resourceConfiguration.ConsensusProtocol == ConsensusProtocol.ZooKeeper;

        var hasLocalZooKeeper =
            isZooKeeperConsensus
            && resourceConfiguration.Environments.TryGetValue(
                "KAFKA_ZOOKEEPER_CONNECT",
                out var connectionString
            )
            && connectionString.StartsWith("localhost");

        _ = Guard
            .Argument(resourceConfiguration, nameof(IContainerConfiguration.Image))
            .ThrowIf(
                _ => hasLocalZooKeeper,
                argument => new ArgumentException(message, argument.Name)
            );
    }

    /// <inheritdoc />
    public string CreateStartupScript(
        KafkaConfiguration resourceConfiguration,
        KafkaContainer container
    )
    {
        var additionalAdvertisedListeners = string.Join(
            ",",
            container.AdvertisedListeners ?? Array.Empty<string>()
        );

        var startupScript = new StringWriter();
        startupScript.NewLine = "\n";
        startupScript.WriteLine("#!/bin/bash");
        startupScript.WriteLine(
            "export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://"
                + container.Hostname
                + ":"
                + container.GetMappedPublicPort(KafkaBuilder.KafkaPort)
                + ",BROKER://"
                + container.IpAddress
                + ":"
                + KafkaBuilder.BrokerPort
                + ","
                + additionalAdvertisedListeners
        );
        startupScript.WriteLine("exec /etc/kafka/docker/run");
        return startupScript.ToString();
    }
}
