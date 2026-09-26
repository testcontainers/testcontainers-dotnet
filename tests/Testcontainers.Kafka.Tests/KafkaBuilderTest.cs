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

    [Theory]
    [InlineData("confluentinc/cp-kafka:8.0.0")]
    [InlineData("confluentinc/cp-kafka:latest")]
    [InlineData("confluentinc/cp-kafka:latest-ubi9")]
    [InlineData("confluentinc/cp-kafka:latest.arm64")]
    public void ZooKeeperWithConfluent8ThrowsArgumentException(string image)
    {
        const string message = "ZooKeeper is not supported for Confluent Platform images with versions 8.0.0 and later. Use KRaft instead.";
        ExpectArgEx(message, () => new KafkaBuilder(image).WithZooKeeper().Build());
    }

    [Theory]
    [InlineData("confluentinc/cp-kafka:8.0.0")]
    [InlineData("confluentinc/cp-kafka:latest")]
    [InlineData("confluentinc/cp-kafka:latest-ubi9")]
    [InlineData("confluentinc/cp-kafka:latest.arm64")]
    public void DefaultWithConfluent8DoesNotThrow(string image)
    {
        var exception = Record.Exception(() => new KafkaBuilder(image).Build());
        Assert.Null(exception);
    }

    private static void ExpectArgEx(string message, Action testCode)
    {
        var exception = Assert.Throws<ArgumentException>(testCode);
        Assert.StartsWith(message, exception.Message);
    }
}