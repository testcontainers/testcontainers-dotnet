namespace DotNet.Testcontainers.Containers
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class CouchbaseTestcontainer : TestcontainerDatabase
  {
    private const string CouchbaseCli = "/opt/couchbase/bin/couchbase-cli";

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal CouchbaseTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    /// <remarks>
    /// TODO: The hostname won't work on Azure, ip address won't work on Windows or macOS.
    /// </remarks>
    public override string ConnectionString
      => $"couchbase://{this.IpAddress}";

    /// <summary>
    /// Creates a new bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket to create.</param>
    /// <param name="memory">The amount of memory to allocate to the cache for this bucket, in Megabytes.</param>
    /// <returns>A task that returns the couchbase-cli exit code when it is finished.</returns>
    public async Task<ExecResult> CreateBucket(string bucket, int memory = 128)
    {
      var createBucketCommand = $"{CouchbaseCli} bucket-create -c 127.0.0.1:8091 --username {this.Username} --password {this.Password} --bucket {bucket} --bucket-type couchbase --bucket-ramsize {memory} --enable-flush 1 --bucket-replica 0 --wait";
      return await this.ExecAsync(new[] { "/bin/sh", "-c", createBucketCommand });
    }

    /// <summary>
    /// Flushes a bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket to flush.</param>
    /// <returns>A task that returns the couchbase-cli exit code when it is finished.</returns>
    public Task<ExecResult> FlushBucket(string bucket)
    {
      var flushBucketCommand = $"yes | {CouchbaseCli} bucket-flush -c 127.0.0.1:8091 --username {this.Username} --password {this.Password} --bucket {bucket}";
      return this.ExecAsync(new[] { "/bin/sh", "-c", flushBucketCommand });
    }
  }
}
