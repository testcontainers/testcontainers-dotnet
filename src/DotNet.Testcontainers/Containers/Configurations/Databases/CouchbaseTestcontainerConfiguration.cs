namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class CouchbaseTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    /// <summary>
    /// Couchbase docker needs to setup over web user interface in first use.
    /// Tests may need already initialized Couchbase.
    /// This customized docker image initialize Couchbase with environment via couchbase-cli.
    /// You can configure following environments for customize your setup. Values are defaults over image.
    /// Source code : https://github.com/mustafaonuraydin/couchbase-docker-image-for-testcontainers
    /// <code>
    /// ENV SERVICES "data,index,query,fts,analytics,eventing"
    /// ENV CLUSTER_RAMSIZE 1024
    /// ENV CLUSTER_INDEX_RAMSIZE 512
    /// ENV CLUSTER_EVENTING_RAMSIZE 256
    /// ENV CLUSTER_FTS_RAMSIZE 256
    /// ENV CLUSTER_ANALYTICS_RAMSIZE 1024
    /// ENV INDEX_STORAGE_SETTING "memopt"
    /// ENV BUCKET_NAME "Sample"
    /// ENV BUCKET_TYPE "couchbase"
    /// ENV BUCKET_RAMSIZE 128
    /// ENV USERNAME "Administrator"
    /// ENV PASSWORD "password"
    /// ENV REST_PORT 8091
    /// ENV CAPI_PORT 8092
    /// ENV QUERY_PORT 8093
    /// ENV FTS_PORT 8094
    /// ENV MEMCACHED_SSL_PORT 11207
    /// ENV MEMCACHED_PORT 11210
    /// ENV SSL_REST_PORT 18091
    /// </code>
    /// </summary>
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

    public CouchbaseTestcontainerConfiguration() : base(CouchbaseImage, BootstrapHttpPort, BootstrapHttpPort)
    {
      this.OutputConsumer = Consume.RedirectStdoutAndStderrToStream(this.stderr, this.stdout);
      this.WaitStrategy = Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, WaitStrategyMessage);
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
      set => this.Environments["CLUSTER_RAMSIZE"] = Validate(nameof(this.ClusterRamSize), value, DefaultClusterRamSize);
    }

    public string ClusterIndexRamSize
    {
      get => this.Environments["CLUSTER_INDEX_RAMSIZE"];
      set => this.Environments["CLUSTER_INDEX_RAMSIZE"] = Validate(nameof(this.ClusterIndexRamSize), value, DefaultClusterIndexRamSize);
    }

    public string ClusterEventingRamSize
    {
      get => this.Environments["CLUSTER_EVENTING_RAMSIZE"];
      set => this.Environments["CLUSTER_EVENTING_RAMSIZE"] = Validate(nameof(this.ClusterEventingRamSize), value, DefaultClusterEventingRamSize);
    }

    public string ClusterFtsRamSize
    {
      get => this.Environments["CLUSTER_FTS_RAMSIZE"];
      set => this.Environments["CLUSTER_FTS_RAMSIZE"] = Validate(nameof(this.ClusterFtsRamSize), value, DefaultClusterFtsRamSize);
    }

    public string ClusterAnalyticsRamSize
    {
      get => this.Environments["CLUSTER_ANALYTICS_RAMSIZE"];
      set => this.Environments["CLUSTER_ANALYTICS_RAMSIZE"] = Validate(nameof(this.ClusterAnalyticsRamSize), value, DefaultClusterAnalyticsRamSize);
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

    public override IOutputConsumer OutputConsumer { get; }

    public override IWaitForContainerOS WaitStrategy { get; }

    private static string Validate(string propertyName, string memory, int minimum)
    {
      if (int.Parse(memory) < minimum)
      {
        throw new ArgumentOutOfRangeException(propertyName, $"Couchbase {propertyName} ram size can not be less than {minimum} MB.");
      }

      return memory;
    }
  }
}
