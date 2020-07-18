namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using System.IO;
  using Abstractions;
  using OutputConsumers;
  using WaitStrategies;

  public sealed class CouchbaseTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string CouchbaseImage = "mustafaonuraydin/couchbase-testcontainer:6.0.0";
    private const string WaitStrategyMessage = "couchbase-dev started";

    private const string DefaultIndexStorage = "memopt";

    private const int DefaultClusterRamSize = 1024;
    private const int DefaultClusterIndexRamSize = 512;
    private const int DefaultClusterEventingRamSize = 256;
    private const int DefaultClusterFtsRamSize = 256;
    private const int DefaultClusterAnalyticsRamSize = 1024;

    private const int BootstrapHttpPort = 8091;

    private Stream OutStream = new MemoryStream();
    private Stream ErrorStream = new MemoryStream();


    public override IOutputConsumer OutputConsumer => Consume.RedirectStdoutAndStderrToStream(this.OutStream, this.ErrorStream);

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, WaitStrategyMessage);

    public CouchbaseTestcontainerConfiguration(string username, string password, string bucket = "Sample", string bucketType = "couchbase", int bucketRamSize = 512)
      : base(CouchbaseImage, BootstrapHttpPort, BootstrapHttpPort)
    {
      this.Username = username;
      this.Password = password;
      this.Port = BootstrapHttpPort;
      this.Environments["ADMIN_NAME"] = username;
      this.Environments["PASSWORD"] = password;
      this.Environments["INITIAL_BUCKET_NAME"] = bucket;
      this.Environments["INITIAL_BUCKET_PASSWORD"] = password;
      this.Environments["INITIAL_BUCKET_TYPE"] = bucketType;
      this.Environments["INITIAL_BUCKET_RAMSIZE"] = bucketRamSize.ToString();
      this.InitializeDefaults();
    }

    public CouchbaseTestcontainerConfiguration ClusterRamSize(int clusterRamSize)
    {
      ValidateMinimumRequirement(nameof(clusterRamSize),clusterRamSize,DefaultClusterRamSize);
      this.Environments["CLUSTER_RAMSIZE"] = clusterRamSize.ToString();
      return this;
    }

    public CouchbaseTestcontainerConfiguration ClusterIndexRamSize(int clusterIndexRamSize)
    {
      ValidateMinimumRequirement(nameof(clusterIndexRamSize),clusterIndexRamSize,DefaultClusterIndexRamSize);
      this.Environments["CLUSTER_INDEX_RAMSIZE"] = clusterIndexRamSize.ToString();
      return this;
    }

    public CouchbaseTestcontainerConfiguration ClusterEventingRamSize(int clusterEventingRamSize)
    {
      ValidateMinimumRequirement(nameof(clusterEventingRamSize),clusterEventingRamSize,DefaultClusterEventingRamSize);
      this.Environments["CLUSTER_EVENTING_RAMSIZE"] = clusterEventingRamSize.ToString();
      return this;
    }

    public CouchbaseTestcontainerConfiguration ClusterFtsRamSize(int clusterFtsRamSize)
    {
      ValidateMinimumRequirement(nameof(clusterFtsRamSize),clusterFtsRamSize,DefaultClusterFtsRamSize);
      this.Environments["CLUSTER_EVENTING_RAMSIZE"] = clusterFtsRamSize.ToString();
      return this;
    }

    public CouchbaseTestcontainerConfiguration ClusterAnalyticsRamSize(int clusterAnalyticsRamSize)
    {
      ValidateMinimumRequirement(nameof(clusterAnalyticsRamSize),clusterAnalyticsRamSize,DefaultClusterAnalyticsRamSize);
      this.Environments["CLUSTER_EVENTING_RAMSIZE"] = clusterAnalyticsRamSize.ToString();
      return this;
    }

    private static void ValidateMinimumRequirement(string name, int memory, int minimum)
    {
      if (memory < minimum)
      {
        throw new ArgumentOutOfRangeException(name,"Couchbase "+name+" ram size can not be less than "+minimum+" MB.");
      }
    }

    private void InitializeDefaults()
    {
      this.Environments["CLUSTER_RAMSIZE"] = DefaultClusterRamSize.ToString();
      this.Environments["CLUSTER_INDEX_RAMSIZE"] = DefaultClusterIndexRamSize.ToString();
      this.Environments["CLUSTER_EVENTING_RAMSIZE"] = DefaultClusterEventingRamSize.ToString();
      this.Environments["CLUSTER_FTS_RAMSIZE"] = DefaultClusterFtsRamSize.ToString();
      this.Environments["CLUSTER_ANALYTICS_RAMSIZE"] = DefaultClusterAnalyticsRamSize.ToString();
      this.Environments["INDEX_STORAGE_SETTING"] = DefaultIndexStorage;
    }

  }
}
