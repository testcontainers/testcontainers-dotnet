namespace DotNet.Testcontainers.Containers.Modules.Databases
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public sealed class CouchbaseTestcontainer : TestcontainerDatabase
  {
    private const string couchbaseCli = "/opt/couchbase/bin/couchbase-cli";

    internal CouchbaseTestcontainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    // TODO: The hostname won't work on Azure, ip address won't work on Windows or macOS?
    public override string ConnectionString => $"couchbase://{this.IpAddress}";

    /// <summary>
    /// Creates a new bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket to create.</param>
    /// <param name="memory">The amount of memory to allocate to the cache for this bucket, in Megabytes.</param>
    /// <returns>A task that returns the couchbase-cli exit code when it is finished.</returns>
    public Task<long> CreateBucket(string bucket, int memory = 128)
    {
      var createBucketCommand = $"{couchbaseCli} bucket-create -c localhost:8091 --username {this.Username} --password {this.Password} --bucket {bucket} --bucket-type couchbase --bucket-ramsize {memory} --enable-flush 1 --bucket-replica 0 --wait";
      return this.ExecAsync(new[] { "/bin/sh", "-c", createBucketCommand });
    }

    /// <summary>
    /// Flushes a bucket
    /// </summary>
    /// <param name="bucket">The name of the bucket to flush.</param>
    /// <returns>A task that returns the couchbase-cli exit code when it is finished.</returns>
    public Task<long> FlushBucket(string bucket)
    {
      var flushBucketCommand = $"yes | {couchbaseCli} bucket-flush -c localhost:8091 --username {this.Username} --password {this.Password} --bucket {bucket}";
      return this.ExecAsync(new[] { "/bin/sh", "-c", flushBucketCommand });
    }
  }
}
