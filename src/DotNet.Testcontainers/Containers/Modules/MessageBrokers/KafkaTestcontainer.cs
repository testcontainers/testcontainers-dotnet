namespace DotNet.Testcontainers.Containers.Modules.MessageBrokers
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;
  using DotNet.Testcontainers.Containers.Configurations;

  public sealed class KafkaTestcontainer : HostedServiceContainer
  {
    internal KafkaTestcontainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public string BootstrapServers => $"{this.Hostname}:{this.Port}";

    /// <summary>
    /// Creates a new topic.
    /// </summary>
    /// <param name="topicName">The name of the topic to create.</param>
    /// <param name="partitions">Count of the topic partitions.</param>
    /// <param name="replicationFactor">Count of the topic raplication factor</param>
    /// <param name="zooKeeperUrl">The url of ZooKeeper</param>
    /// <returns>A task that returns the kafka-cli exit code when it is finished.</returns>
    public Task<long> CreateTopic(string topicName, int partitions = 3, int replicationFactor = 1, string zooKeeperUrl = "localhost:2181")
    {
      var createTopicCommand = $"/bin/kafka-topics --create --zookeeper {zooKeeperUrl} --topic {topicName} --partitions {partitions} --replication-factor {replicationFactor}";
      return this.ExecAsync(new[] {"/bin/sh", "-c", createTopicCommand});
    }
  }
}
