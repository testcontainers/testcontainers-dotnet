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

    // # --8<-- [start:UsePulsarContainer]
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

        await using var client = clientBuilder.Build();

        await using var producer = client.NewProducer(Schema.String)
            .Topic(topic)
            .Create();

        await using var consumer = client.NewConsumer(Schema.String)
            .Topic(topic)
            .SubscriptionName(name)
            .Create();

        // When
        _ = await producer.Send(helloPulsar)
            .ConfigureAwait(true);

        var message = await consumer.Receive()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloPulsar, Encoding.Default.GetString(message.Data));
    }
    // # --8<-- [end:UsePulsarContainer]

    // # --8<-- [start:CreatePulsarContainer]
    [UsedImplicitly]
    public sealed class PulsarDefaultConfiguration : PulsarContainerTest
    {
        public PulsarDefaultConfiguration()
            : base(new PulsarBuilder().Build(), false)
        {
        }
    }
    // # --8<-- [end:CreatePulsarContainer]

    [UsedImplicitly]
    public sealed class PulsarAuthConfiguration : PulsarContainerTest
    {
        public PulsarAuthConfiguration()
            : base(new PulsarBuilder().WithAuthentication().Build(), true)
        {
        }
    }

    [UsedImplicitly]
    public sealed class PulsarV4Configuration : PulsarContainerTest
    {
        public PulsarV4Configuration()
            : base(new PulsarBuilder().WithImage("apachepulsar/pulsar:4.0.2").Build(), false)
        {
        }
    }

    [UsedImplicitly]
    public sealed class PulsarV4AuthConfiguration : PulsarContainerTest
    {
        public PulsarV4AuthConfiguration()
            : base(new PulsarBuilder().WithImage("apachepulsar/pulsar:4.0.2").WithAuthentication().Build(), true)
        {
        }
    }
}