namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Globalization;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerMessageBrokerConfiguration" />
  [PublicAPI]
  public class KafkaTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    private const string KafkaImage = "confluentinc/cp-kafka:6.0.5";

    private const string StartupScriptPath = "/testcontainers_start.sh";

    private const int KafkaPort = 9092;

    private const int BrokerPort = 9093;

    private const int ZookeeperPort = 2181;

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaTestcontainerConfiguration" /> class.
    /// </summary>
    public KafkaTestcontainerConfiguration()
      : this(KafkaImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public KafkaTestcontainerConfiguration(string image)
      : base(image, KafkaPort)
    {
      // Use two listeners with different names, it will force Kafka to communicate with itself via internal
      // listener when KAFKA_INTER_BROKER_LISTENER_NAME is set, otherwise Kafka will try to use the advertised listener.
      this.Environments.Add("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,PLAINTEXT:PLAINTEXT");
      this.Environments.Add("KAFKA_LISTENERS", $"PLAINTEXT://0.0.0.0:{this.DefaultPort},BROKER://0.0.0.0:{BrokerPort}");
      this.Environments.Add("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER");
      this.Environments.Add("KAFKA_BROKER_ID", "1");
      this.Environments.Add("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1");
      this.Environments.Add("KAFKA_OFFSETS_TOPIC_NUM_PARTITIONS", "1");
      this.Environments.Add("KAFKA_LOG_FLUSH_INTERVAL_MESSAGES", long.MaxValue.ToString(CultureInfo.InvariantCulture));
      this.Environments.Add("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0");
      this.Environments.Add("KAFKA_ZOOKEEPER_CONNECT", $"localhost:{ZookeeperPort}");
    }

    /// <summary>
    /// Gets the command.
    /// </summary>
    public virtual string[] Command { get; }
      = { "/bin/sh", "-c", $"while [ ! -f {StartupScriptPath} ]; do sleep 0.1; done; {StartupScriptPath}" };

    /// <summary>
    /// Gets the startup callback.
    /// </summary>
    public virtual Func<IRunningDockerContainer, CancellationToken, Task> StartupCallback
      => (container, ct) =>
      {
        const char lf = '\n';
        var startupScript = new StringBuilder();
        startupScript.Append("#!/bin/sh");
        startupScript.Append(lf);
        startupScript.Append($"echo 'clientPort={ZookeeperPort}' > zookeeper.properties");
        startupScript.Append(lf);
        startupScript.Append("echo 'dataDir=/var/lib/zookeeper/data' >> zookeeper.properties");
        startupScript.Append(lf);
        startupScript.Append("echo 'dataLogDir=/var/lib/zookeeper/log' >> zookeeper.properties");
        startupScript.Append(lf);
        startupScript.Append("zookeeper-server-start zookeeper.properties &");
        startupScript.Append(lf);
        startupScript.Append($"export KAFKA_ADVERTISED_LISTENERS='PLAINTEXT://{container.Hostname}:{container.GetMappedPublicPort(this.DefaultPort)},BROKER://localhost:{BrokerPort}'");
        startupScript.Append(lf);
        startupScript.Append(". /etc/confluent/docker/bash-config");
        startupScript.Append(lf);
        startupScript.Append("/etc/confluent/docker/configure");
        startupScript.Append(lf);
        startupScript.Append("/etc/confluent/docker/launch");
        return container.CopyFileAsync(StartupScriptPath, Encoding.Default.GetBytes(startupScript.ToString()), 0x1ff, ct: ct);
      };

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
