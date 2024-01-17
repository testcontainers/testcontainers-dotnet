namespace Testcontainers.PubSub;

public sealed class PubSubContainerTest : IAsyncLifetime
{
    private readonly PubSubContainer _pubSubContainer = new PubSubBuilder().Build();

    public Task InitializeAsync()
    {
        return _pubSubContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _pubSubContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SubTopicReturnsPubMessage()
    {
        // Given
        const string helloPubSub = "Hello, PubSub!";

        const string projectId = "hello-pub-sub";

        const string topicId = "hello-topic";

        const string subscriptionId = "hello-subscription";

        var topicName = new TopicName(projectId, topicId);

        var subscriptionName = new SubscriptionName(projectId, subscriptionId);

        var message = new PubsubMessage();
        message.Data = ByteString.CopyFromUtf8(helloPubSub);

        var publisherClientBuilder = new PublisherServiceApiClientBuilder();
        publisherClientBuilder.Endpoint = _pubSubContainer.GetEmulatorEndpoint();
        publisherClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        var subscriberClientBuilder = new SubscriberServiceApiClientBuilder();
        subscriberClientBuilder.Endpoint = _pubSubContainer.GetEmulatorEndpoint();
        subscriberClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        // When
        var publisher = await publisherClientBuilder.BuildAsync()
            .ConfigureAwait(true);

        _ = await publisher.CreateTopicAsync(topicName)
            .ConfigureAwait(true);

        var subscriber = await subscriberClientBuilder.BuildAsync()
            .ConfigureAwait(true);

        _ = await subscriber.CreateSubscriptionAsync(subscriptionName, topicName, null, 60)
            .ConfigureAwait(true);

        _ = await publisher.PublishAsync(topicName, new[] { message })
            .ConfigureAwait(true);

        var response = await subscriber.PullAsync(subscriptionName, 1)
            .ConfigureAwait(true);

        await subscriber.AcknowledgeAsync(subscriptionName, response.ReceivedMessages.Select(receivedMessage => receivedMessage.AckId))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloPubSub, response.ReceivedMessages.Single().Message.Data.ToStringUtf8());
    }
}