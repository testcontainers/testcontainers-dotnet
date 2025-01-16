namespace Testcontainers.Kafka;

public sealed class KafkaContainerNetworkTest : IAsyncLifetime
{
    private const string Message = "Message produced by kafkacat";

    private const string Listener = "kafka:19092";

    private const string DataFilePath = "/data/msgs.txt";

    private readonly INetwork _network;

    private readonly IContainer _kafkaContainer;

    private readonly IContainer _kCatContainer;

    public KafkaContainerNetworkTest()
    {
        _network = new NetworkBuilder()
            .Build();

        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:6.1.9")
            .WithNetwork(_network)
            .WithListener(Listener)
            .Build();

        _kCatContainer = new ContainerBuilder()
            .WithImage("confluentinc/cp-kafkacat:6.1.9")
            .WithNetwork(_network)
            .WithEntrypoint(CommonCommands.SleepInfinity)
            .WithResourceMapping(Encoding.Default.GetBytes(Message), DataFilePath)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync()
            .ConfigureAwait(false);

        await _kCatContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _kafkaContainer.StartAsync()
            .ConfigureAwait(false);

        await _kCatContainer.StartAsync()
            .ConfigureAwait(false);

        await _network.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task ConsumesProducedKafkaMessage()
    {
        _ = await _kCatContainer.ExecAsync(new[] { "kafkacat", "-b", Listener, "-t", "msgs", "-P", "-l", DataFilePath })
            .ConfigureAwait(true);

        var execResult = await _kCatContainer.ExecAsync(new[] { "kafkacat", "-b", Listener, "-C", "-t", "msgs", "-c", "1" })
            .ConfigureAwait(true);

        Assert.Equal(Message, execResult.Stdout.Trim());
    }
}