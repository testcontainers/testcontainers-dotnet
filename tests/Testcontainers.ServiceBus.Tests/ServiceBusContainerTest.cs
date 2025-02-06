namespace Testcontainers.ServiceBus;

public abstract class ServiceBusContainerTest : IAsyncLifetime
{
    private readonly ServiceBusContainer _serviceBusContainer;

    private ServiceBusContainerTest(ServiceBusContainer serviceBusContainer)
    {
        _serviceBusContainer = serviceBusContainer;
    }

    public Task InitializeAsync()
    {
        return _serviceBusContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _serviceBusContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReceiveMessageReturnsSentMessage()
    {
        // Given
        const string helloServiceBus = "Hello, Service Bus!";

        // By default, the emulator uses the following configuration:
        // https://learn.microsoft.com/en-us/azure/service-bus-messaging/test-locally-with-service-bus-emulator?tabs=automated-script#interact-with-the-emulator.

        // Upload a custom configuration before the container starts using the
        // `WithResourceMapping(string, string)` API or one of its overloads:
        // `WithResourceMapping("Config.json", "/ServiceBus_Emulator/ConfigFiles/")`.
        const string queueName = "queue.1";

        var message = new ServiceBusMessage(helloServiceBus);

        await using var client = new ServiceBusClient(_serviceBusContainer.GetConnectionString());

        var sender = client.CreateSender(queueName);

        var receiver = client.CreateReceiver(queueName);

        // When
        await sender.SendMessageAsync(message)
            .ConfigureAwait(true);

        var receivedMessage = await receiver.ReceiveMessageAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloServiceBus, receivedMessage.Body.ToString());
    }

    [UsedImplicitly]
    public sealed class ServiceBusDefaultMsSqlConfiguration : ServiceBusContainerTest
    {
        public ServiceBusDefaultMsSqlConfiguration()
            : base(new ServiceBusBuilder()
                .WithAcceptLicenseAgreement(true)
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ServiceBusCustomMsSqlConfiguration : ServiceBusContainerTest, IClassFixture<DatabaseFixture>
    {
        public ServiceBusCustomMsSqlConfiguration(DatabaseFixture fixture)
            : base(new ServiceBusBuilder()
                .WithAcceptLicenseAgreement(true)
                .WithMsSqlContainer(fixture.Network, fixture.Container, DatabaseFixture.DatabaseNetworkAlias)
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class DatabaseFixture
    {
        public DatabaseFixture()
        {
            Network = new NetworkBuilder()
                .Build();

            Container = new MsSqlBuilder()
                .WithNetwork(Network)
                .WithNetworkAliases(DatabaseNetworkAlias)
                .Build();
        }

        public static string DatabaseNetworkAlias => ServiceBusBuilder.DatabaseNetworkAlias;

        public INetwork Network { get; }

        public MsSqlContainer Container { get; }
    }
}