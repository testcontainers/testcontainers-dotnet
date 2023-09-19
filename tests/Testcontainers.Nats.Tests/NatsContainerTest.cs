namespace Testcontainers.Nats;

public sealed class NatsContainerTest : IAsyncLifetime
{
    private readonly NatsContainer _natsContainer = new NatsBuilder().Build();

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
    public async Task ContainerIsStartedWithCorrectParameters()
    {
        using var client = new ConnectionFactory()
            .CreateConnection(_natsContainer.GetConnectionString());

        Assert.Equal(ConnState.CONNECTED, client.State);
        Assert.True(client.ServerInfo.JetStreamAvailable);

        using var monitorClient = new HttpClient();
        monitorClient.BaseAddress = new Uri(_natsContainer.GetManagementEndpoint());

        using var response = await monitorClient.GetAsync("/healthz");
        var s = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PubSubSendsAndReturnsMessages()
    {
        using var client = new ConnectionFactory()
            .CreateConnection(_natsContainer.GetConnectionString());

        using ISyncSubscription subSync = client.SubscribeSync("greet.pam");
        client.Publish("greet.pam", Encoding.UTF8.GetBytes("hello pam 1"));

        var msg = subSync.NextMessage(1000);
        var text = Encoding.UTF8.GetString(msg.Data);


        Assert.Equal("hello pam 1", text);
        await client.DrainAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task BuilderShouldBuildWithUserNameAndPassword()
    {
        var encodedPassword = Uri.EscapeDataString("??&&testpass");
        var encodedUsername = Uri.EscapeDataString("??&&test");

        var builder = new NatsBuilder()
            .WithUsername("??&&test")
            .WithPassword("??&&testpass");
        await using var container = builder.Build();

        await container.StartAsync();

        var uri = new Uri(container.GetConnectionString());

        Assert.Equal($"{encodedUsername}:{encodedPassword}", uri.UserInfo);

        using var client = new ConnectionFactory()
            .CreateConnection(_natsContainer.GetConnectionString());

        Assert.Equal(ConnState.CONNECTED, client.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void BuilderShouldFailWithOnlyUserNameOrPassword()
    {
        var builder = new NatsBuilder().WithUsername("??&&test");
        Assert.Throws<ArgumentException>(() => builder.Build());

        builder = new NatsBuilder().WithPassword("??&&test");
        Assert.Throws<ArgumentException>(() => builder.Build());
    }
}