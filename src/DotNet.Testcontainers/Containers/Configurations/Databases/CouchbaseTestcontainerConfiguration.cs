namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using System.IO;
  using Abstractions;
  using OutputConsumers;
  using WaitStrategies;

  public sealed class CouchbaseTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string CouchbaseImage = "mustafaonuraydin/couchbase-testcontainer:6.5.1";
    private const string WaitStrategyMessage = "couchbase-dev started";

    private const int DefaultClusterRamSize = 1024;
    private const int DefaultClusterIndexRamSize = 512;
    private const int DefaultClusterEventingRamSize = 256;
    private const int DefaultClusterFtsRamSize = 256;
    private const int DefaultClusterAnalyticsRamSize = 1024;

    private const int BootstrapHttpPort = 8091;

    private readonly MemoryStream stdout = new MemoryStream();
    private readonly MemoryStream stderr = new MemoryStream();

    public override IOutputConsumer OutputConsumer => Consume.RedirectStdoutAndStderrToStream(this.stdout, this.stderr);

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, WaitStrategyMessage);

    public CouchbaseTestcontainerConfiguration() : base(CouchbaseImage, BootstrapHttpPort, BootstrapHttpPort)
    {
    }

    public override string Username
    {
      get => this.Environments["USERNAME"];
      set => this.Environments["USERNAME"] = value;
    }

    public override string Password
    {
      get => this.Environments["PASSWORD"];
      set => this.Environments["PASSWORD"] = value;
    }

    public string BucketName
    {
      get => this.Environments["BUCKET_NAME"];
      set => this.Environments["BUCKET_NAME"] = value;
    }
    public string BucketType
    {
      get => this.Environments["BUCKET_TYPE"];
      set => this.Environments["BUCKET_TYPE"] = value;
    }

    public string BucketRamSize
    {
      get => this.Environments["BUCKET_RAMSIZE"];
      set => this.Environments["BUCKET_RAMSIZE"] = value;
    }

    public string ClusterRamSize
    {
      get => this.Environments["CLUSTER_RAMSIZE"];
      set => this.Environments["CLUSTER_RAMSIZE"] = Validate(nameof(this.ClusterRamSize),value,DefaultClusterRamSize);
    }

    public string ClusterIndexRamSize
    {
      get => this.Environments["CLUSTER_INDEX_RAMSIZE"];
      set => this.Environments["CLUSTER_INDEX_RAMSIZE"] = Validate(nameof(this.ClusterIndexRamSize),value,DefaultClusterIndexRamSize);
    }

    public string ClusterEventingRamSize
    {
      get => this.Environments["CLUSTER_EVENTING_RAMSIZE"];
      set => this.Environments["CLUSTER_EVENTING_RAMSIZE"] = Validate(nameof(this.ClusterEventingRamSize),value,DefaultClusterEventingRamSize);
    }

    public string ClusterFtsRamSize
    {
      get => this.Environments["CLUSTER_FTS_RAMSIZE"];
      set => this.Environments["CLUSTER_FTS_RAMSIZE"] = Validate(nameof(this.ClusterFtsRamSize),value,DefaultClusterFtsRamSize);
    }

    public string ClusterAnalyticsRamSize
    {
      get => this.Environments["CLUSTER_ANALYTICS_RAMSIZE"];
      set => this.Environments["CLUSTER_ANALYTICS_RAMSIZE"] = Validate(nameof(this.ClusterAnalyticsRamSize), value, DefaultClusterAnalyticsRamSize);
    }

    private static string Validate(string name, string memory, int minimum)
    {
      if (int.Parse(memory) < minimum)
      {
        throw new ArgumentOutOfRangeException(name,$"Couchbase {name} ram size can not be less than {minimum} MB.");
      }
      return memory;
    }

  }
}
