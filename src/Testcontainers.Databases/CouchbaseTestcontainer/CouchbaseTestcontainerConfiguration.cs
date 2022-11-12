namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public sealed class CouchbaseTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    /// <summary>
    /// Couchbase docker needs to setup over web user interface in first use.
    /// Tests may need already initialized Couchbase.
    /// This customized docker image initialize Couchbase with environment via couchbase-cli.
    /// You can configure following environments for customize your setup. Values are defaults over image.
    /// Source code : https://github.com/mustafaonuraydin/couchbase-docker-image-for-testcontainers.
    /// <code>
    ///   ENV SERVICES "data,index,query,fts,analytics,eventing"
    ///   ENV CLUSTER_RAMSIZE 1024
    ///   ENV CLUSTER_INDEX_RAMSIZE 512
    ///   ENV CLUSTER_EVENTING_RAMSIZE 256
    ///   ENV CLUSTER_FTS_RAMSIZE 256
    ///   ENV CLUSTER_ANALYTICS_RAMSIZE 1024
    ///   ENV INDEX_STORAGE_SETTING "memopt"
    ///   ENV BUCKET_NAME "Sample"
    ///   ENV BUCKET_TYPE "couchbase"
    ///   ENV BUCKET_RAMSIZE 128
    ///   ENV USERNAME "Administrator"
    ///   ENV PASSWORD "password"
    ///   ENV REST_PORT 8091
    ///   ENV CAPI_PORT 8092
    ///   ENV QUERY_PORT 8093
    ///   ENV FTS_PORT 8094
    ///   ENV MEMCACHED_SSL_PORT 11207
    ///   ENV MEMCACHED_PORT 11210
    ///   ENV SSL_REST_PORT 18091
    /// </code>
    /// </summary>
    private const string CouchbaseImage = "mustafaonuraydin/couchbase-testcontainer:6.5.1";

    private const string WaitUntilMessageIsLogged = "couchbase-dev started";

    private const int DefaultClusterRamSize = 256;

    private const int DefaultClusterIndexRamSize = 256;

    private const int DefaultClusterFtsRamSize = 256;

    private const int DefaultClusterEventingRamSize = 256;

    private const int DefaultClusterAnalyticsRamSize = 1024;

    private const int BootstrapHttpPort = 8091;

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseTestcontainerConfiguration" /> class.
    /// </summary>
    public CouchbaseTestcontainerConfiguration()
      : base(CouchbaseImage, BootstrapHttpPort, BootstrapHttpPort)
    {
      this.OutputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
      this.WaitStrategy = Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, WaitUntilMessageIsLogged);
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
      get
      {
        return this.Environments["CLUSTER_RAMSIZE"];
      }

      set
      {
        ThrowIfMemoryIsLessThanMinimum(nameof(this.ClusterRamSize), value, DefaultClusterRamSize);
        this.Environments["CLUSTER_RAMSIZE"] = value;
      }
    }

    public string ClusterIndexRamSize
    {
      get
      {
        return this.Environments["CLUSTER_INDEX_RAMSIZE"];
      }

      set
      {
        ThrowIfMemoryIsLessThanMinimum(nameof(this.ClusterIndexRamSize), value, DefaultClusterIndexRamSize);
        this.Environments["CLUSTER_INDEX_RAMSIZE"] = value;
      }
    }

    public string ClusterFtsRamSize
    {
      get
      {
        return this.Environments["CLUSTER_FTS_RAMSIZE"];
      }

      set
      {
        ThrowIfMemoryIsLessThanMinimum(nameof(this.ClusterFtsRamSize), value, DefaultClusterFtsRamSize);
        this.Environments["CLUSTER_FTS_RAMSIZE"] = value;
      }
    }

    public string ClusterEventingRamSize
    {
      get
      {
        return this.Environments["CLUSTER_EVENTING_RAMSIZE"];
      }

      set
      {
        ThrowIfMemoryIsLessThanMinimum(nameof(this.ClusterEventingRamSize), value, DefaultClusterEventingRamSize);
        this.Environments["CLUSTER_EVENTING_RAMSIZE"] = value;
      }
    }

    public string ClusterAnalyticsRamSize
    {
      get
      {
        return this.Environments["CLUSTER_ANALYTICS_RAMSIZE"];
      }

      set
      {
        ThrowIfMemoryIsLessThanMinimum(nameof(this.ClusterAnalyticsRamSize), value, DefaultClusterAnalyticsRamSize);
        this.Environments["CLUSTER_ANALYTICS_RAMSIZE"] = value;
      }
    }

    /// <inheritdoc />
    public override string Username
    {
      get => this.Environments["USERNAME"];
      set => this.Environments["USERNAME"] = value;
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["PASSWORD"];
      set => this.Environments["PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy { get; }

    private static void ThrowIfMemoryIsLessThanMinimum(string propertyName, string value, int minimumMemoryInMb)
    {
      var succeeded = int.TryParse(value, out var memoryInMb);

      if (!succeeded)
      {
        throw new ArgumentException($"{value} is not an integer.", propertyName);
      }

      if (memoryInMb < minimumMemoryInMb)
      {
        throw new ArgumentOutOfRangeException(propertyName, $"Couchbase {propertyName} ram size can not be less than {minimumMemoryInMb} MB.");
      }
    }
  }
}
