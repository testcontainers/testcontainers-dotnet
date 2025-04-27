namespace Testcontainers.Kafka;

public sealed class KafkaContainerRegistryTest : IAsyncLifetime
{
    private const string Schema = @"
    {
        ""$schema"": ""http://json-schema.org/draft-04/schema#"",
        ""title"": ""User"",
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {
            ""FirstName"": {
                ""type"": [""null"", ""string""]
            },
            ""LastName"": {
                ""type"": [""null"", ""string""]
            }
        }
    }";

    private const ushort RestPort = 8085;

    private const string SchemaRegistryNetworkAlias = "schema-registry";

    private const string Listener = "kafka:19092";

    private readonly INetwork _network;

    private readonly KafkaContainer _kafkaContainer;

    private readonly IContainer _schemaRegistryContainer;

    public KafkaContainerRegistryTest()
    {
        _network = new NetworkBuilder()
            .Build();

        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:6.1.9")
            .WithNetwork(_network)
            .WithListener(Listener)
            .Build();

        _schemaRegistryContainer = new ContainerBuilder()
            .WithImage("confluentinc/cp-schema-registry:6.1.9")
            .WithPortBinding(RestPort, true)
            .WithNetwork(_network)
            .WithNetworkAliases(SchemaRegistryNetworkAlias)
            .WithEnvironment("SCHEMA_REGISTRY_LISTENERS", "http://0.0.0.0:" + RestPort)
            .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_SECURITY_PROTOCOL", "PLAINTEXT")
            .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", "PLAINTEXT://" + Listener)
            .WithEnvironment("SCHEMA_REGISTRY_HOST_NAME", SchemaRegistryNetworkAlias)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(RestPort).ForPath("/subjects")))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync()
            .ConfigureAwait(false);

        await _schemaRegistryContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _kafkaContainer.StartAsync()
            .ConfigureAwait(false);

        await _schemaRegistryContainer.StartAsync()
            .ConfigureAwait(false);

        await _network.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task ConsumerReturnsProducerMessage()
    {
        // Given
        const string topic = "user";

        var subject = SubjectNameStrategy.Topic.ConstructValueSubjectName(topic);

        var bootstrapServer = _kafkaContainer.GetBootstrapAddress();

        var producerConfig = new ProducerConfig();
        producerConfig.BootstrapServers = bootstrapServer;

        var consumerConfig = new ConsumerConfig();
        consumerConfig.BootstrapServers = bootstrapServer;
        consumerConfig.GroupId = "sample-consumer";
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;

        var message = new Message<string, User>();
        message.Value = new User("John", "Doe");

        var schemaRegistryConfig = new SchemaRegistryConfig();
        schemaRegistryConfig.Url = new UriBuilder(Uri.UriSchemeHttp, _schemaRegistryContainer.Hostname, _schemaRegistryContainer.GetMappedPublicPort(RestPort)).ToString();

        // When
        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        _ = await schemaRegistry.RegisterSchemaAsync(subject, new Schema(Schema, SchemaType.Json))
            .ConfigureAwait(true);

        using var producer = new ProducerBuilder<string, User>(producerConfig)
            .SetValueSerializer(new JsonSerializer<User>(schemaRegistry))
            .Build();

        _ = await producer.ProduceAsync(topic, message)
            .ConfigureAwait(true);

        using var consumer = new ConsumerBuilder<string, User>(consumerConfig)
            .SetValueDeserializer(new JsonDeserializer<User>().AsSyncOverAsync())
            .Build();

        consumer.Subscribe(topic);

        var result = consumer.Consume(TimeSpan.FromSeconds(15));

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);
    }

    private record User(string FirstName, string LastName);
}