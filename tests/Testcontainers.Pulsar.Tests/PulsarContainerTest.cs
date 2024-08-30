namespace Testcontainers.Pulsar;

public abstract class PulsarContainerTest : IAsyncLifetime
{
    private readonly PulsarContainer _pulsarContainer;

    private readonly bool _authenticationEnabled;

    private PulsarContainerTest(PulsarContainer pulsarContainer, bool authenticationEnabled)
    {
        _pulsarContainer = pulsarContainer;
        _authenticationEnabled = authenticationEnabled;
    }

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

        var clientBuilder = PulsarClient.Builder().ServiceUrl(new Uri(_pulsarContainer.GetBrokerAddress()));

        if (_authenticationEnabled)
        {
            var authToken = await _pulsarContainer.CreateAuthenticationTokenAsync(Timeout.InfiniteTimeSpan);
            _ = clientBuilder.Authentication(new TokenAuthentication(authToken));
        }

        var client = clientBuilder.Build();

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

    // CreatePulsarContainer {
    [UsedImplicitly]
    public sealed class PulsarDefaultConfiguration : PulsarContainerTest
    {
        public PulsarDefaultConfiguration()
            : base(new PulsarBuilder().Build(), false)
        {
        }
    }
    // }

    [UsedImplicitly]
    public sealed class PulsarAuthConfiguration : PulsarContainerTest
    {
        public PulsarAuthConfiguration()
            : base(new PulsarBuilder().WithAuthentication().Build(), true)
        {
        }
    }
}