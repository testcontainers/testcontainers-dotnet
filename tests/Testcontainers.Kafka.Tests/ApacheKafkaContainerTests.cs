namespace Testcontainers.Kafka;

public class ApacheKafkaContainerTests
{
    private const string ApacheKafkaImage = "apache/kafka:3.7.2";
    private const string ApacheNativeKafkaImage = "apache/kafka-native:3.9.0";

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WorksWithApacheImage()
    {
        await using var kafkaContainer = new KafkaBuilder()
            .WithImage(ApacheKafkaImage)
            .Build();
        await kafkaContainer.StartAsync();

        await TestHelper.AssertKafkaProducerConsumer(kafkaContainer);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WorksWithNativeImage()
    {
        await using var kafkaContainer = new KafkaBuilder()
            .WithImage(ApacheNativeKafkaImage)
            .Build();
        await kafkaContainer.StartAsync();
        
        await TestHelper.AssertKafkaProducerConsumer(kafkaContainer);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DoesNotAllowToUseEmbeddedZookeeper()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await using var kafkaContainer = new KafkaBuilder()
                .WithImage(ApacheKafkaImage)
                .WithZookeeper()
                .Build();
            await kafkaContainer.StartAsync();
        });
    }
}