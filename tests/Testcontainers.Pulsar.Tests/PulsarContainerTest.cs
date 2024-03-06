namespace Testcontainers.Pulsar;

public sealed class PulsarContainerTest : IAsyncLifetime
{
    private readonly PulsarContainer _pulsarContainer = new PulsarBuilder().Build();

    public Task InitializeAsync()
    {
        return _pulsarContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _pulsarContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConsumerReturnsProducerMessage()
    {
        const string topic = "sample";

        await using var client = PulsarClient.Builder().ServiceUrl(_pulsarContainer.GetServiceUrl()).Build();
        await using var producer = client.NewProducer(Schema.String).Topic(topic).Create();
        await using var consumer = client.NewConsumer(Schema.String).Topic(topic).SubscriptionName("sample-subscription").Create();

        var message = Guid.NewGuid().ToString("D");

        _ = await producer.Send(message).ConfigureAwait(true);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(15));
        var result = await consumer.Receive(cts.Token).ConfigureAwait(true);

        Assert.Equal(message, result.Value());
    }
}