using DotNet.Testcontainers.Images;

namespace Testcontainers.Kafka.Vendors;

internal class ConfluentVendor : IKafkaVendor
{
    public KafkaImageVendor ImageVendor => KafkaImageVendor.Confluent;

    public KafkaConsensusProtocol DefaultConsensusProtocol => KafkaConsensusProtocol.Zookeeper; // for backward compatibility

    public bool ImageBelongsToVendor(IImage image)
    {
        return image.Repository.Contains("confluentinc");
    }

    public void ValidateConfigurationAndThrow(KafkaConfiguration configuration)
    {
        if (configuration.ConsensusProtocol == KafkaConsensusProtocol.KRaft &&
            configuration.Image.Repository.Contains("confluentinc") &&
            configuration.Image.MatchVersion(v => v.Major < 7))
        {
            throw new ArgumentException("KRaft is not supported for Confluent Platform image versions less than 7.0.0. " +
                                        "Please use WithImage() to specify a newer version of Confluent Kafka image.");
        }
    }

    public string GetStartupScript(StartupScriptContext context)
    {
        var container = context.Container;
        var additionalAdvertisedListeners = string.Join(",", container.AdvertisedListeners ?? Array.Empty<string>());

        switch (context.Configuration.ConsensusProtocol)
        {
            case KafkaConsensusProtocol.Zookeeper:
            {
                if (context.Configuration.ExternalZookeeperConnectionString is null)
                {
                    // starting local Zookeeper instance
                    return
$"""
 #!/bin/bash
 
 echo 'clientPort={KafkaBuilder.ZookeeperPort}' > zookeeper.properties
 echo 'dataDir=/var/lib/zookeeper/data' >> zookeeper.properties
 zookeeper-server-start zookeeper.properties &
 
 export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaBuilder.KafkaPort)},BROKER://{container.IpAddress}:{KafkaBuilder.BrokerPort},{additionalAdvertisedListeners}
 echo '' > /etc/confluent/docker/ensure
 exec /etc/confluent/docker/run
 """;
                }

                return
$"""
 #!/bin/bash

 export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaBuilder.KafkaPort)},BROKER://{container.IpAddress}:{KafkaBuilder.BrokerPort},{additionalAdvertisedListeners}
 exec /etc/confluent/docker/run
 """;
            }
            case KafkaConsensusProtocol.KRaft:
            {
                return
$"""
 #!/bin/bash

 export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(KafkaBuilder.KafkaPort)},BROKER://{container.IpAddress}:{KafkaBuilder.BrokerPort},{additionalAdvertisedListeners}
 exec /etc/confluent/docker/run
 """;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(context.Configuration.ConsensusProtocol), "Unknown consensus protocol");
        }
    }
}