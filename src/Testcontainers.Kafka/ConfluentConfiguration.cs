namespace Testcontainers.Kafka;

/// <inheritdoc cref="IKafkaVendorConfiguration" />
internal sealed class ConfluentConfiguration : IKafkaVendorConfiguration
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
    public static IKafkaVendorConfiguration Instance { get; }
        = new ConfluentConfiguration();

    /// <inheritdoc />
    public KafkaVendor Vendor
        => KafkaVendor.Confluent;

    /// <inheritdoc />
    public ConsensusProtocol GetConsensusProtocol(IImage image)
    {
        return IsEarlierThanVersion8(image) ? ConsensusProtocol.ZooKeeper : ConsensusProtocol.KRaft;
    }

    /// <inheritdoc />
    public bool IsImageFromVendor(IImage image)
    {
        return image.Repository.Contains("confluentinc");
    }

    /// <inheritdoc />
    public void Validate(KafkaConfiguration resourceConfiguration)
    {
        const string kraftMessage = "KRaft is not supported for Confluent Platform images with versions earlier than 7.0.0. Use ZooKeeper instead.";

        const string zooKeeperMessage = "ZooKeeper is not supported for Confluent Platform images with versions 8.0.0 and later. Use KRaft instead.";

        Predicate<KafkaConfiguration> isUnsupportedKRaftImage = value => value.ConsensusProtocol == ConsensusProtocol.KRaft
            && IsImageFromVendor(value.Image) && value.Image.MatchVersion(v => v.Major < 7);

        Predicate<KafkaConfiguration> isUnsupportedZooKeeperImage = value => value.ConsensusProtocol == ConsensusProtocol.ZooKeeper
            && IsImageFromVendor(value.Image) && IsVersion8OrLater(value.Image);

        _ = Guard.Argument(resourceConfiguration, nameof(IContainerConfiguration.Image))
            .ThrowIf(argument => isUnsupportedKRaftImage(argument.Value), argument => new ArgumentException(kraftMessage, argument.Name))
            .ThrowIf(argument => isUnsupportedZooKeeperImage(argument.Value), argument => new ArgumentException(zooKeeperMessage, argument.Name));
    }

    /// <inheritdoc />
    public string CreateStartupScript(KafkaConfiguration resourceConfiguration, KafkaContainer container)
    {
        var advertisedListeners = new List<string>();
        advertisedListeners.Add("PLAINTEXT://" + container.Hostname + ":" + container.GetMappedPublicPort(KafkaBuilder.KafkaPort));
        advertisedListeners.Add("BROKER://" + container.IpAddress + ":" + KafkaBuilder.BrokerPort);
        advertisedListeners.AddRange(container.AdvertisedListeners ?? Array.Empty<string>());

        var isZooKeeperConsensus = resourceConfiguration.ConsensusProtocol == ConsensusProtocol.ZooKeeper;

        var hasLocalZooKeeper = isZooKeeperConsensus && resourceConfiguration.Environments.TryGetValue("KAFKA_ZOOKEEPER_CONNECT", out var connectionString) && connectionString.StartsWith("localhost");

        var startupScript = new StringWriter();
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

        startupScript.WriteLine("export KAFKA_ADVERTISED_LISTENERS=" + string.Join(",", advertisedListeners));
        startupScript.WriteLine("exec /etc/confluent/docker/run");
        return startupScript.ToString();
    }

    /// <summary>
    /// Determines whether the image tag is a Confluent Platform version earlier
    /// than 8.0.0, which still ships ZooKeeper. Tags without a version, such
    /// as <c>latest</c> or custom tags, and digest-only references return
    /// <c>false</c>, so the default consensus protocol falls back to KRaft.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <returns><c>true</c> if the tag is a version earlier than 8.0.0; otherwise, <c>false</c>.</returns>
    private static bool IsEarlierThanVersion8(IImage image)
    {
        return image.MatchVersion(v => v.Major < 8);
    }

    /// <summary>
    /// Determines whether the image tag is known to point to Confluent Platform
    /// 8.0.0 or later, which removed ZooKeeper. This includes <c>latest</c> and
    /// its suffixed variants, such as <c>latest-ubi9</c> and <c>latest.arm64</c>.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <returns><c>true</c> if the tag points to 8.0.0 or later; otherwise, <c>false</c>.</returns>
    private static bool IsVersion8OrLater(IImage image)
    {
        return image.MatchVersion(v => v.Major >= 8) || image.MatchVersion((string tag) => tag != null && tag.StartsWith("latest", StringComparison.Ordinal));
    }
}