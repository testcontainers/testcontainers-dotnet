using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.MsSql;

namespace Testcontainers.ServiceBus;

public class ServiceBusContainerWithMsSqlContainerTest : IAsyncLifetime
{
    private static readonly INetwork Network = new NetworkBuilder().Build();
    
    private static readonly MsSqlContainer MsSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
        .WithNetwork(Network)
        .WithNetworkAliases("sql-server")
        .Build();
    
    private readonly ServiceBusContainer _serviceBusContainer = new ServiceBusBuilder()
        .WithNetwork(Network)
        .WithAcceptLicenseAgreement(true)
        .WithMsSqlContainer(MsSqlContainer, "sql-server")
        .Build();
    
    public async Task InitializeAsync()
    {
        await MsSqlContainer.StartAsync();
        await _serviceBusContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await MsSqlContainer.DisposeAsync();
        await _serviceBusContainer.DisposeAsync();
        await Network.DisposeAsync();
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
}