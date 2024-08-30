namespace Testcontainers.Pulsar;

public abstract class PulsarContainerTest : IAsyncLifetime
{
    private readonly PulsarContainer _pulsarContainer;

    private PulsarContainerTest(PulsarContainer pulsarContainer)
    {
        _pulsarContainer = pulsarContainer;
    }

    protected abstract Task<IPulsarClient> CreateClientAsync(CancellationToken ct = default);

    // UsePulsarContainer {
    public Task InitializeAsync()
    {
        return _pulsarContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _pulsarContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task ConsumerReceivesSendMessage()
    {
        // Given
        const string helloPulsar = "Hello, Pulsar!";

        var topic = $"persistent://public/default/{Guid.NewGuid():D}";

        var name = Guid.NewGuid().ToString("D");

        await using var client = await CreateClientAsync()
            .ConfigureAwait(true);

        await using var producer = client.NewProducer(Schema.String)
            .Topic(topic)
            .Create();

        await using var consumer = client.NewConsumer(Schema.String)
            .Topic(topic)
            .SubscriptionName(name)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        // When
        _ = await producer.Send(helloPulsar)
            .ConfigureAwait(true);

        var message = await consumer.Receive()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloPulsar, Encoding.Default.GetString(message.Data));
    }
    // }

    [UsedImplicitly]
    public sealed class PulsarDefaultConfiguration : PulsarContainerTest
    {
        // CreatePulsarContainer {
        public PulsarDefaultConfiguration()
            : base(new PulsarBuilder().Build())
        {
        }
        // }

        protected override Task<IPulsarClient> CreateClientAsync(CancellationToken ct = default)
        {
            return Task.FromResult(PulsarClient.Builder().ServiceUrl(new Uri(_pulsarContainer.GetBrokerAddress())).Build());
        }
    }

    [UsedImplicitly]
    public sealed class PulsarAuthConfiguration : PulsarContainerTest
    {
        public PulsarAuthConfiguration()
            : base(new PulsarBuilder().WithAuthentication().Build())
        {
        }

        protected override async Task<IPulsarClient> CreateClientAsync(CancellationToken ct = default)
        {
            var authToken = await _pulsarContainer.CreateAuthenticationTokenAsync(Timeout.InfiniteTimeSpan, ct)
                .ConfigureAwait(false);

            return PulsarClient.Builder().ServiceUrl(new Uri(_pulsarContainer.GetBrokerAddress())).Authentication(new TokenAuthentication(authToken)).Build();
        }
    }
}