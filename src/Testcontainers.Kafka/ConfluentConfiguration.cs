namespace Testcontainers.Kafka;

/// <inheritdoc cref="KafkaVendorConfiguration" />
internal sealed class ConfluentConfiguration : KafkaVendorConfiguration
{
    static ConfluentConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfluentConfiguration" /> class.
    /// </summary>
    private ConfluentConfiguration()
    {
    }

    /// <summary>
    /// Gets the singleton instance of the Confluent vendor configuration.
    /// </summary>
    public static KafkaVendorConfiguration Instance { get; }
        = new ConfluentConfiguration();

    /// <inheritdoc />
    public override KafkaVendor Vendor
        => KafkaVendor.Confluent;

    /// <inheritdoc />
    public override ConsensusProtocol ConsensusProtocol
        => ConsensusProtocol.ZooKeeper;

    /// <inheritdoc />
    public override bool IsImageFromVendor(IImage image)
    {
        return image.Repository.Contains("confluentinc");
    }

    /// <inheritdoc />
    public override void Validate(KafkaConfiguration resourceConfiguration)
    {
        const string message = "KRaft is not supported for Confluent Platform images with versions earlier than 7.0.0.";

        Predicate<KafkaConfiguration> isUnsupportedImage = value => value.ConsensusProtocol == ConsensusProtocol.KRaft
            && IsImageFromVendor(value.Image) && value.Image.MatchVersion(v => v.Major < 7);

        _ = Guard.Argument(resourceConfiguration, nameof(IContainerConfiguration.Image))
            .ThrowIf(argument => isUnsupportedImage(argument.Value), argument => new ArgumentException(message, argument.Name));
    }

    /// <inheritdoc />
    public override string CreateStartupScript(KafkaConfiguration resourceConfiguration, KafkaContainer container)
    {
        var additionalAdvertisedListeners = string.Join(",", container.AdvertisedListeners ?? Array.Empty<string>());

        var isZooKeeperConsensus = resourceConfiguration.ConsensusProtocol == ConsensusProtocol.ZooKeeper;

        var hasLocalZooKeeper = isZooKeeperConsensus && resourceConfiguration.Environments.TryGetValue("KAFKA_ZOOKEEPER_CONNECT", out var connectionString) && connectionString.StartsWith("localhost");

        using var startupScript = new StringWriter();
        startupScript.NewLine = "\n";
        startupScript.WriteLine("#!/bin/bash");

        if (isZooKeeperConsensus && hasLocalZooKeeper)
        {
            startupScript.WriteLine("echo '' > /etc/confluent/docker/ensure");
            startupScript.WriteLine("echo 'clientPort=" + KafkaBuilder.ZooKeeperPort + "' > zookeeper.properties");
            startupScript.WriteLine("echo 'dataDir=/var/lib/zookeeper/data' >> zookeeper.properties");
            startupScript.WriteLine("echo 'dataLogDir=/var/lib/zookeeper/log' >> zookeeper.properties");
            startupScript.WriteLine("zookeeper-server-start zookeeper.properties &");
        }

        startupScript.WriteLine("export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://" + container.Hostname + ":" + container.GetMappedPublicPort(KafkaBuilder.KafkaPort) + ",BROKER://" + container.IpAddress + ":" + KafkaBuilder.BrokerPort + "," + additionalAdvertisedListeners);
        startupScript.WriteLine("exec /etc/confluent/docker/run");
        return startupScript.ToString();
    }
}