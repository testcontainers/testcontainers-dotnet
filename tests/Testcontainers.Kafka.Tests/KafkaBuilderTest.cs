namespace Testcontainers.Kafka;

public sealed class KafkaBuilderTests
{
    [Fact]
    public void KRaftWithConfluentPre7ThrowsArgumentException()
    {
        const string message = "KRaft is not supported for Confluent Platform images with versions earlier than 7.0.0.";
        ExpectArgEx(message, () => new KafkaBuilder("confluentinc/cp-kafka:6.1.9").WithKRaft().Build());
    }

    [Fact]
    public void ZooKeeperWithApacheKafkaImageThrowsArgumentException()
    {
        const string message = "Local ZooKeeper is not supported for Apache Kafka images. Configure an external ZooKeeper.";
        ExpectArgEx(message, () => new KafkaBuilder("apache/kafka:3.9.1").WithZooKeeper().Build());
    }

    private static void ExpectArgEx(string message, Action testCode)
    {
        var exception = Assert.Throws<ArgumentException>(testCode);
        Assert.StartsWith(message, exception.Message);
    }
}