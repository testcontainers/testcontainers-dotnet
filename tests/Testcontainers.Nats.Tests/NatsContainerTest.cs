namespace Testcontainers.Nats;

public abstract class NatsContainerTest : IAsyncLifetime
{
    private readonly NatsContainer _natsContainer;

    private NatsContainerTest(NatsContainer natsContainer)
    {
        _natsContainer = natsContainer;
    }

    public Task InitializeAsync()
    {
        return _natsContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _natsContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthcheckReturnsHttpStatusCodeOk()
    {
        // Given
        using var client = new HttpClient();
        client.BaseAddress = new Uri(_natsContainer.GetManagementEndpoint());

        // When
        using var response = await client.GetAsync("/healthz")
            .ConfigureAwait(true);

        var jsonStatusString = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
        public void ThrowsExceptionIfUsernameIsMissing()
        {
            Assert.Throws<ArgumentException>(() => new NatsBuilder().WithPassword("password").Build());
        }

        [Fact]
        public void ThrowsExceptionIfPasswordIsMissing()
        {
            Assert.Throws<ArgumentException>(() => new NatsBuilder().WithUsername("username").Build());
        }
    }
}