using Azure.Messaging.ServiceBus.Administration;

namespace Testcontainers.ServiceBus.Tests;

public abstract class ServiceBusAdministrationClientTest : IAsyncLifetime
{
    private readonly ServiceBusContainer _serviceBusContainer;

    private ServiceBusAdministrationClientTest(ServiceBusContainer serviceBusContainer)
    {
        _serviceBusContainer = serviceBusContainer;
    }

    public async ValueTask InitializeAsync()
    {
        await _serviceBusContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _serviceBusContainer.DisposeAsync();
    }


    [UsedImplicitly]
    public sealed class ServiceBusDefaultMsSqlConfiguration : ServiceBusAdministrationClientTest
    {
        public ServiceBusDefaultMsSqlConfiguration()
            : base(new ServiceBusBuilder(TestSession.GetImageFromDockerfile())
                .WithAcceptLicenseAgreement(true)
                .Build())
        {
        }
    }


    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateQueueViaServiceBusAdministrationClient()
    {
        // Given
        const string helloServiceBus = "Hello, Service Bus!";
        var queueName = $"sample-queue-{Guid.NewGuid()}";

        // By default, the emulator uses the following configuration:
        // https://learn.microsoft.com/en-us/azure/service-bus-messaging/test-locally-with-service-bus-emulator?tabs=automated-script#interact-with-the-emulator.

        var message = new ServiceBusMessage(helloServiceBus);

        await using var client = new ServiceBusClient(_serviceBusContainer.GetConnectionString());
        var adminClient = new ServiceBusAdministrationClient(_serviceBusContainer.GetHttpConnectionString());

        var sender = client.CreateSender(queueName);

        var receiver = client.CreateReceiver(queueName);

        // When
        await adminClient.CreateQueueAsync(new CreateQueueOptions(queueName), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await sender.SendMessageAsync(message, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var receivedMessage = await receiver.ReceiveMessageAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloServiceBus, receivedMessage.Body.ToString());
    }
}
