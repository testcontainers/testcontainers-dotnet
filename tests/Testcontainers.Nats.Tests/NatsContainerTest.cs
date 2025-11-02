namespace Testcontainers.Nats;

public abstract class NatsContainerTest : IAsyncLifetime
{
    private readonly NatsContainer _natsContainer;

    private NatsContainerTest(NatsContainer natsContainer)
    {
        _natsContainer = natsContainer;
    }

    public async ValueTask InitializeAsync()
    {
        await _natsContainer.StartAsync()
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
    public async Task HealthcheckReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_natsContainer.GetManagementEndpoint());

        // When
        using var httpResponse = await httpClient.GetAsync("/healthz", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var jsonStatusString = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("{\"status\":\"ok\"}", jsonStatusString);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetStringReturnsPublishString()
    {
        // Given
        var subject = Guid.NewGuid().ToString("D");

        var message = Guid.NewGuid().ToString("D");

        using var client = new ConnectionFactory().CreateConnection(_natsContainer.GetConnectionString());

        using var subscription = client.SubscribeSync(subject);

        // When
        client.Publish(subject, Encoding.Default.GetBytes(message));

        var actualMessage = Encoding.Default.GetString(subscription.NextMessage().Data);

        // Then
        Assert.Equal(message, actualMessage);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _natsContainer.DisposeAsync();
    }

    [UsedImplicitly]
    public sealed class NatsDefaultConfiguration : NatsContainerTest
    {
        public NatsDefaultConfiguration()
            : base(new NatsBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class NatsAuthConfiguration : NatsContainerTest
    {
        public NatsAuthConfiguration()
            : base(new NatsBuilder().WithUsername("%username!").WithPassword("?password&").Build())
        {
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public void ThrowsExceptionIfUsernameIsMissing()
        {
            Assert.Throws<ArgumentException>(() => new NatsBuilder().WithPassword("password").Build());
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public void ThrowsExceptionIfPasswordIsMissing()
        {
            Assert.Throws<ArgumentException>(() => new NatsBuilder().WithUsername("username").Build());
        }
    }
}