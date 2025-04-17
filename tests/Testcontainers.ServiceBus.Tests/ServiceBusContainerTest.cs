namespace Testcontainers.ServiceBus;

public abstract class ServiceBusContainerTest : IAsyncLifetime
{
    private readonly ServiceBusContainer _serviceBusContainer;

    private ServiceBusContainerTest(ServiceBusContainer serviceBusContainer)
    {
        _serviceBusContainer = serviceBusContainer;
    }

    protected virtual string QueueName => "queue.1";

    // # --8<-- [start:UseServiceBusContainer]
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

        var message = new ServiceBusMessage(helloServiceBus);

        await using var client = new ServiceBusClient(_serviceBusContainer.GetConnectionString());

        var sender = client.CreateSender(QueueName);

        var receiver = client.CreateReceiver(QueueName);

        // When
        await sender.SendMessageAsync(message)
            .ConfigureAwait(true);

        var receivedMessage = await receiver.ReceiveMessageAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloServiceBus, receivedMessage.Body.ToString());
    }
    // # --8<-- [end:UseServiceBusContainer]

    // # --8<-- [start:CreateServiceBusContainer]
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
    // # --8<-- [end:CreateServiceBusContainer]

    [UsedImplicitly]
    public sealed class ServiceBusCustomMsSqlConfiguration : ServiceBusContainerTest, IClassFixture<DatabaseFixture>
    {
        public ServiceBusCustomMsSqlConfiguration(DatabaseFixture fixture)
            : base(new ServiceBusBuilder()
                .WithAcceptLicenseAgreement(true)
                // # --8<-- [start:ReuseExistingMsSqlContainer]
                .WithMsSqlContainer(fixture.Network, fixture.Container, DatabaseFixture.DatabaseNetworkAlias)
                // # --8<-- [end:ReuseExistingMsSqlContainer]
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ServiceBusCustomQueueConfiguration : ServiceBusContainerTest, IClassFixture<DatabaseFixture>
    {
        public ServiceBusCustomQueueConfiguration()
            : base(new ServiceBusBuilder()
                .WithAcceptLicenseAgreement(true)
                // # --8<-- [start:UseCustomConfiguration]
                .WithConfig("custom-queue-config.json")
                // # --8<-- [end:UseCustomConfiguration]
                .Build())
        {
        }

        protected override string QueueName => "custom-queue.1";
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