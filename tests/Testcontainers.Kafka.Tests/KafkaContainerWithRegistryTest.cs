using System.Diagnostics;
using System.Threading;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Testcontainers.Kafka;

public class KafkaContainerWithRegistryTest : IAsyncLifetime
{
    private INetwork _network;
    private KafkaContainer _kafkaContainer;
    private IContainer _kafkaSchemaRegistry;

    const string schema = @"
        {
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""$id"": ""http://example.com/product.schema.json"",
            ""title"": ""User"",
            ""description"": ""A User"",
            ""type"": ""object"",
            ""properties"": {
                ""age"": {
                  ""description"": ""The age of the user"",
                  ""type"": ""integer""
                },
                ""lastname"": {
                  ""description"": ""Last name of the user"",
                  ""type"": ""string""
                },
                ""firstname"": {
                  ""description"": ""First name of the user"",
                  ""type"": ""string""
                }
            },
            ""required"": [""firstname"", ""lastname""]
        }";
    
    public async Task InitializeAsync()
    {
        _network = new NetworkBuilder().Build();
        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka")
            .WithNetwork(_network)
            .WithListener("kafka:19092")
            .Build();
        
        _kafkaSchemaRegistry = new ContainerBuilder()
            .WithImage("confluentinc/cp-schema-registry:7.8.0")
            .DependsOn(_kafkaContainer)
            .WithPortBinding(8085, true)
            .WithNetworkAliases("schemaregistry")
            .WithNetwork(_network)
            .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", "PLAINTEXT://kafka:19092")
            .WithEnvironment("SCHEMA_REGISTRY_LISTENERS", "http://0.0.0.0:8085")
            .WithEnvironment("SCHEMA_REGISTRY_HOST_NAME", "schemaregistry")
            .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_SECURITY_PROTOCOL", "PLAINTEXT")
            .WithWaitStrategy(
                 Wait.ForUnixContainer()
                     .UntilHttpRequestIsSucceeded(request => request.ForPath("/subjects")
                         .ForPort(8085))
                    
            )
            .Build();
        
        await _kafkaContainer.StartAsync();
        await _kafkaSchemaRegistry.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _kafkaContainer.DisposeAsync().AsTask(),
            _kafkaSchemaRegistry.DisposeAsync().AsTask()
        );
    }
    
    /// <summary>
    /// Test the usage of the Kafka container with the schema registry.
    /// </summary>
    [Fact]
    public async Task TestUsageWithSchemaRegistry()
    {
        const string topicName = "user-topic";
        var subject = SubjectNameStrategy.Topic.ConstructValueSubjectName(topicName, null);
        
        var bootstrapServers = this._kafkaContainer.GetBootstrapAddress()
            .Replace("PLAINTEXT://", "", StringComparison.OrdinalIgnoreCase);
        
        var jsonSerializerConfig = new JsonSerializerConfig
        {
            BufferBytes = 100,
        };
        
        var schemaRegistryUrl = $"http://localhost:{_kafkaSchemaRegistry.GetMappedPublicPort(8085)}";

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = schemaRegistryUrl,
        };
        // Init Kafka producer to send a message
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = $"test-client-{DateTime.Now.Ticks}",
        };
        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        
        var schemaId = await schemaRegistry.RegisterSchemaAsync(subject, new Schema(schema, SchemaType.Json));
        
        using var producer = new ProducerBuilder<string, User>(producerConfig)
            .SetValueSerializer(new JsonSerializer<User>(schemaRegistry, jsonSerializerConfig))
            .Build();
        
        await Assert.ThrowsAsync<SchemaRegistryException>(async () =>
        {
            try
            {
                var user = new User { Name = "value", Age = 30 };
                await producer.ProduceAsync(topicName, new Message<string, User> { Value = user });
            }
            catch (Exception e)
            {
                Assert.True(e is ProduceException<string, User>);
                Debug.Assert(e.InnerException != null, "e.InnerException != null");
                throw e.InnerException;
            }
        });
    }
}
