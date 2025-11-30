namespace Testcontainers.Pulsar;

public abstract class PulsarContainerTest : IAsyncLifetime
{
    private static readonly IReadOnlyDictionary<string, string> MemorySettings = new Dictionary<string, string> { { "PULSAR_MEM", "-Xms256m -Xmx512m" } };

    private readonly PulsarContainer _pulsarContainer;

    private readonly bool _authenticationEnabled;

    private PulsarContainerTest(PulsarContainer pulsarContainer, bool authenticationEnabled)
    {
        _pulsarContainer = pulsarContainer;
        _authenticationEnabled = authenticationEnabled;
    }

    // # --8<-- [start:UsePulsarContainer]
    public async ValueTask InitializeAsync()
    {
        await _pulsarContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConsumerReceivesSendMessage()
    {
        // Given
        const string helloPulsar = "Hello, Pulsar!";

        var topic = $"persistent://public/default/{Guid.NewGuid():D}";

        var name = Guid.NewGuid().ToString("D");

        var clientBuilder = PulsarClient.Builder().ServiceUrl(new Uri(_pulsarContainer.GetBrokerAddress()));

        if (_authenticationEnabled)
        {
            var authToken = await _pulsarContainer.CreateAuthenticationTokenAsync(Timeout.InfiniteTimeSpan, TestContext.Current.CancellationToken);
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
        _ = await consumer.OnStateChangeTo(ConsumerState.Active, TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await producer.OnStateChangeTo(ProducerState.Connected, TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await producer.Send(helloPulsar, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var message = await consumer.Receive(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloPulsar, Encoding.Default.GetString(message.Data));
    }
    // # --8<-- [end:UsePulsarContainer]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _pulsarContainer.DisposeAsync();
    }

    // # --8<-- [start:CreatePulsarContainer]
    [UsedImplicitly]
    public sealed class PulsarDefaultConfiguration : PulsarContainerTest
    {
        public PulsarDefaultConfiguration()
            : base(new PulsarBuilder(TestSession.GetImageFromDockerfile())
                .WithEnvironment(MemorySettings)
                .Build(), false)
        {
        }
    }
    // # --8<-- [end:CreatePulsarContainer]

    [UsedImplicitly]
    public sealed class PulsarAuthConfiguration : PulsarContainerTest
    {
        public PulsarAuthConfiguration()
            : base(new PulsarBuilder(TestSession.GetImageFromDockerfile())
                .WithAuthentication()
                .WithEnvironment(MemorySettings)
                .Build(), true)
        {
        }
    }

    [UsedImplicitly]
    public sealed class PulsarV4Configuration : PulsarContainerTest
    {
        public PulsarV4Configuration()
            : base(new PulsarBuilder(TestSession.GetImageFromDockerfile())
                .WithImage("apachepulsar/pulsar:4.0.2")
                .WithEnvironment(MemorySettings)
                .Build(), false)
        {
        }
    }

    [UsedImplicitly]
    public sealed class PulsarV4AuthConfiguration : PulsarContainerTest
    {
        public PulsarV4AuthConfiguration()
            : base(new PulsarBuilder(TestSession.GetImageFromDockerfile())
                .WithImage("apachepulsar/pulsar:4.0.2")
                .WithAuthentication()
                .WithEnvironment(MemorySettings)
                .Build(), true)
        {
        }
    }
}