namespace Testcontainers.Kafka;

public class ConfluentKafkaContainerTests
{
    private const string OldConfluentImage = "confluentinc/cp-kafka:6.1.9";
    private const string NewConfluentImage = "confluentinc/cp-kafka:7.5.1";

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WorksWithDefaultSettings()
    {
        await using var kafkaContainer = new KafkaBuilder().Build();
        await kafkaContainer.StartAsync();

        await TestHelper.AssertKafkaProducerConsumer(kafkaContainer);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WorksWithInternalZookeeper()
    {
        await using var kafkaContainer = new KafkaBuilder()
            .WithImage(NewConfluentImage)
            .WithZookeeper()
            .Build();
        await kafkaContainer.StartAsync();

        await TestHelper.AssertKafkaProducerConsumer(kafkaContainer);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WorksWithKRaft()
    {
        await using var kafkaContainer = new KafkaBuilder()
            .WithImage(NewConfluentImage)
            .WithKRaft()
            .Build();
        await kafkaContainer.StartAsync();

        await TestHelper.AssertKafkaProducerConsumer(kafkaContainer);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DoesNotAllowKRaftToBeUsedWithOldImage()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await using var kafkaContainer = new KafkaBuilder()
                .WithImage(OldConfluentImage)
                .WithKRaft()
                .Build();
            await kafkaContainer.StartAsync();
        });
    }
}