
namespace Testcontainers.PubSub.Tests;

public class PubSubContainerTests : IAsyncLifetime
{
  private readonly PubSubContainer _pubSubContainer = new PubSubBuilder().Build();

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task PublishSubscribeTest()
  {
    const string projectId = "testProj";
    const string topicName = "testTopic";
    const string subscriptionName = "test-sub";

    var (publisher, subscriber) = await _pubSubContainer.CreatePublisherAndSubscriber();
    var (topic,subscription) = await PubSubHelper.EnsureTopicAndSubscription(publisher, subscriber, projectId, topicName, subscriptionName);

    var dataToSent = new testData();

    await publisher.PublishAsync(topic, dataToSent.ToPubsubMessage().AsList());

    var messages = await subscriber.PullAsyncImmediatelly(subscription);

    Assert.Single(messages.ReceivedMessages);

    await subscriber.AcknowledgeAllMessages(subscription, messages.ReceivedMessages);

    var receivedObject = messages.ReceivedMessages.FirstOrDefault().GetStringData().AsObject<testData>();

    Assert.Equal(receivedObject.Id,dataToSent.Id);
  }

  public Task InitializeAsync()
  {
    return _pubSubContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _pubSubContainer.DisposeAsync().AsTask();
  }

  public class testData
  {
    public string Id { get; set; } = Guid.NewGuid().ToString();
  }
}
