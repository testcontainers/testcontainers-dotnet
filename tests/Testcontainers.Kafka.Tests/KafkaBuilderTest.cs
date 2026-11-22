namespace Testcontainers.Kafka;

public sealed class KafkaBuilderTests
{
    [Theory]
    [InlineData("")]
    [InlineData("confluent-v8_2_4")]
    [InlineData("apache-v4_1_1")]
    [InlineData("apache-native-v4_4_0")]
    public void ImplicitVendorWithKnownImageDoesNotThrow(string stage)
    {
        var exception = Record.Exception(() => new KafkaBuilder(TestSession.GetImageFromDockerfile(stage: stage)).Build());
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(KafkaVendor.ApacheSoftwareFoundation)]
    [InlineData(KafkaVendor.Confluent)]
    public void ExplicitVendorWithCustomImageDoesNotThrow(KafkaVendor vendor)
    {
        var exception = Record.Exception(() => new KafkaBuilder("custom/kafka:1.0.0").WithVendor(vendor).Build());
        Assert.Null(exception);
    }

    [Fact]
    public void KRaftWithConfluentPre7ThrowsArgumentException()
    {
        const string message = "KRaft is not supported for Confluent Platform images with versions earlier than 7.0.0. Use ZooKeeper instead.";
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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CustomEntrypointAndCommandArePreserved()
    {
        // Given
        var entrypoint = new[] { "/bin/bash", "-c" };

        var command = new[] { "trap : TERM INT; sleep infinity & wait" };

        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder(Guid.NewGuid()).Build();

        await using var container = new KafkaBuilder(TestSession.GetImageFromDockerfile(stage: "apache-v4_1_1"))
            .WithKRaft()
            .WithEntrypoint(entrypoint)
            .WithCommand(new OverwriteEnumerable<string>(command))
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();

        // When
        await container.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await dockerClient.Containers.InspectContainerAsync(container.Id, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(entrypoint, response.Config.Entrypoint);
        Assert.Equal(command, response.Config.Cmd);
    }

    private static void ExpectArgEx(string message, Action testCode)
    {
        var exception = Assert.Throws<ArgumentException>(testCode);
        Assert.StartsWith(message, exception.Message);
    }
}
