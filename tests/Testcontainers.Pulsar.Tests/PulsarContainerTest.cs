using SharpPulsar;
using SharpPulsar.Builder;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace Testcontainers.Pulsar;

public sealed class PulsarContainerTest : IAsyncLifetime
{
    private readonly PulsarContainer _pulsarContainer = new PulsarBuilder().Build();

    public async Task InitializeAsync()
    {
        await _pulsarContainer.StartAsync();
        _output.WriteLine("Start Test Container");
        await AwaitPortReadiness($"http://127.0.0.1:{_pulsarContainer.GetMappedPublicPort(8080)}/metrics/");
        _output.WriteLine("metrics");
    }
    private static async ValueTask AwaitPortReadiness(string address)
    {
        var waitTries = 20;

        using var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true
        };

        using var client = new HttpClient(handler);

        while (waitTries > 0)
        {
            try
            {
                await client.GetAsync(address).ConfigureAwait(false);
                return;
            }
            catch
            {
                waitTries--;
                await Task.Delay(5000).ConfigureAwait(false);
            }
        }

        throw new Exception("Unable to confirm Pulsar has initialized");
    }
    public Task DisposeAsync()
    {
        return _pulsarContainer.DisposeAsync().AsTask();
    }
    private readonly ITestOutputHelper _output;
    public PulsarContainerTest(ITestOutputHelper output) 
    {
        _output = output;
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConsumerReturnsProducerMessage()
    {
        const string topic = "sample";
        var url = $"pulsar://127.0.0.1:{_pulsarContainer.GetMappedPublicPort(6650)}";
        var clientConfig = new PulsarClientConfigBuilder()
                .EnableTransaction(true)
                .ServiceUrl(url);

        //pulsar actor system
        var pulsarSystem = PulsarSystem.GetInstance(actorSystemName: "test");

        var pulsarClient = await pulsarSystem.NewClient(clientConfig);
        var consumer = await pulsarClient
                .NewConsumerAsync(new ConsumerConfigBuilder<byte[]>()
                .Topic(topic)
                .ForceTopicCreation(true)
                .SubscriptionName($"sub-{Guid.NewGuid()}")
                .IsAckReceiptEnabled(true));

       var producer = await pulsarClient
                .NewProducerAsync(new ProducerConfigBuilder<byte[]>()
                .Topic(topic));


        for (var i = 0; i < 50; i++)
        {
            var data = Encoding.UTF8.GetBytes($"test container -{i}");
            var id = await producer.NewMessage().Value(data).SendAsync();
            _output.WriteLine(JsonSerializer.Serialize(id, new JsonSerializerOptions { WriteIndented = true }));
        }
        var numbers = 0;
        for (var i = 0; i < 48; i++)
        {
            var message = (Message<byte[]>)await consumer.ReceiveAsync();
            if (message != null)
            {
                await consumer.AcknowledgeAsync(message);
                var res = Encoding.UTF8.GetString(message.Data);
                _output.WriteLine(message.GetMessageId().ToString(), message.ProducerName, message.Topic, res);
                numbers++;
            }
        }
        
        Assert.True(numbers > 10);
    }
}