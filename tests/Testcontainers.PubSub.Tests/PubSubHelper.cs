

namespace Testcontainers.PubSub.Tests
{
  public static class PubSubHelper
  {
    public static async Task<(TopicName, SubscriptionName)> EnsureTopicAndSubscription(
      PublisherServiceApiClient publisher, SubscriberServiceApiClient subscriber, string projectId, string topicName,
      string subscriptionName)
    {
      var topic = TopicName.FromProjectTopic(projectId, topicName);
      var subscription = new SubscriptionName(projectId, subscriptionName);

      await publisher.CreateTopicAsync(topic);

      await subscriber.CreateSubscriptionAsync(new Subscription
      {
        SubscriptionName = subscription,
        TopicAsTopicName = topic,
        PushConfig = null,
        AckDeadlineSeconds = 0,
        RetainAckedMessages = false,
        Labels = {{"", ""},},
        EnableMessageOrdering = false,
        ExpirationPolicy = new ExpirationPolicy(),
        Filter = "",
        DeadLetterPolicy = null,
        RetryPolicy = new RetryPolicy(),
        Detached = false,
        EnableExactlyOnceDelivery = false,
        TopicMessageRetentionDuration = new Duration(),
        BigqueryConfig = null,
        State = Subscription.Types.State.Unspecified,
      });

      return (topic, subscription);
    }

    public static async Task<(PublisherServiceApiClient publisher, SubscriberServiceApiClient subscriber)>
      CreatePublisherAndSubscriber(this PubSubContainer container)
    {
      var publisherBuilder = new PublisherServiceApiClientBuilder()
      {
        ChannelCredentials = ChannelCredentials.Insecure,
        Endpoint = container.GetEmulatorEndpoint(),
      };

      var subscriberBuilder = new SubscriberServiceApiClientBuilder()
      {
        ChannelCredentials = ChannelCredentials.Insecure,
        Endpoint = container.GetEmulatorEndpoint(),
      };

      var publisher = await publisherBuilder.BuildAsync();
      var subscriber = await subscriberBuilder.BuildAsync();

      return (publisher, subscriber);
    }

    public static PubsubMessage ToPubsubMessage<T>(this T data)
    {
      return new PubsubMessage()
      {
        Data = ByteString.CopyFrom(System.Text.Json.JsonSerializer.Serialize(data), System.Text.Encoding.UTF8)
      };
    }

    public static List<PubsubMessage> AsList(this PubsubMessage message)
    {
      return new List<PubsubMessage>()
      {
        message
      };
    }

    public static async Task<PullResponse> PullAsyncImmediatelly(this SubscriberServiceApiClient subscriber,
      SubscriptionName subscription, int maxMessages = 100)
    {
      return await subscriber.PullAsync(new PullRequest
      {
        SubscriptionAsSubscriptionName = subscription,
        ReturnImmediately = true,
        MaxMessages = maxMessages,
      });
    }

    public static async Task AcknowledgeAllMessages(this SubscriberServiceApiClient subscriber,
      SubscriptionName subscription, RepeatedField<ReceivedMessage> messages)
    {
      await subscriber.AcknowledgeAsync(subscription, messages.Select(x => x.AckId));
    }

    public static string GetStringData(this ReceivedMessage message)
    {
      return System.Text.Encoding.UTF8.GetString(message.Message.Data.ToArray());
    }

    public static T AsObject<T>(this string text)
    {
      return System.Text.Json.JsonSerializer.Deserialize<T>(text
        , new JsonSerializerOptions()
        {
          PropertyNameCaseInsensitive = true
        }
      );
    }
  }
}
