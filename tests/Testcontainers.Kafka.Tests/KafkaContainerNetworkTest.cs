using System.Collections.Generic;
using System.Text;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Testcontainers.Kafka;

public sealed class KafkaContainerNetworkTest : IAsyncLifetime
{
    private INetwork _network;
    private KafkaContainer _kafkaContainer;

    private IContainer _kCatContainer;
    public async Task InitializeAsync()
    {
        _network = new NetworkBuilder().Build();
        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka")
            .WithNetwork(_network)
            .WithListener("kafka:19092")
            .Build();

        _kCatContainer = new ContainerBuilder()
            .WithImage("confluentinc/cp-kcat")
            .WithNetwork(_network)
            .WithCommand("-c", "tail -f /dev/null")
            .WithEntrypoint("sh")
            .WithResourceMapping(Encoding.Default.GetBytes("Message produced by kcat"), "/data/msgs.txt")
            .Build();
        
        await _kCatContainer.StartAsync(); 
        await _kafkaContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.WhenAll(
            _kafkaContainer.DisposeAsync().AsTask(), 
            _kCatContainer.DisposeAsync().AsTask()
        );
    }
    
    [Fact]
    public async Task TestUsageWithListener()
    {
        // kcat producer
        await _kCatContainer.ExecAsync(new List<string>()
        {
            "kcat", "-b", "kafka:19092", "-t", "msgs", "-P", "-l", "/data/msgs.txt"
        });
        
        
        // kcat consumer
        var kCatResult = await _kCatContainer.ExecAsync(new List<string>()
        {
            "kcat", "-b", "kafka:19092", "-C", "-t", "msgs", "-c", "1"
        });
        
        Assert.Contains("Message produced by kcat", kCatResult.Stdout);
    }
    
}