using DotNet.Testcontainers.Images;

namespace Testcontainers.Kafka.Vendors;

internal class ApacheKafkaVendor : IKafkaVendor
{
    public KafkaImageVendor ImageVendor => KafkaImageVendor.Apache;
    public KafkaConsensusProtocol DefaultConsensusProtocol => KafkaConsensusProtocol.KRaft;

    public bool ImageBelongsToVendor(IImage image)
    {
        return image.Repository.Contains("apache");
    }

    public void ValidateConfigurationAndThrow(KafkaConfiguration configuration)
    {
        // no validation required
    }

    public string GetStartupScript(StartupScriptContext context)
    {
        var container = context.Container;
        var additionalAdvertisedListeners = string.Join(",", container.AdvertisedListeners ?? Array.Empty<string>());

        if (context.Configuration.ConsensusProtocol == KafkaConsensusProtocol.Zookeeper &&
            context.Configuration.ExternalZookeeperConnectionString is null)
        {
            throw new ArgumentException("Local Zookeeper is not supported by the provided Kafka docker image. " +
                                        "Please specify an external Zookeeper connection string using WithZookeeper().");
        }

        return
$"""
 #!/bin/bash

 export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaBuilder.KafkaPort)},BROKER://{container.IpAddress}:{KafkaBuilder.BrokerPort},{additionalAdvertisedListeners}
 exec /etc/kafka/docker/run
 """;
    }
}