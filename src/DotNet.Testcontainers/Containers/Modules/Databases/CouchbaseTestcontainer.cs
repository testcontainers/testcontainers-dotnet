namespace DotNet.Testcontainers.Containers.Modules.Databases
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public class CouchbaseTestcontainer : TestcontainerDatabase
  {
    private const string couchbaseCli = "/opt/couchbase/bin/couchbase-cli";

    internal CouchbaseTestcontainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"couchbase://{this.Hostname}";

    public async Task<long> CreateNewBucket(string bucket, int memory = 128)
    {
      var createBucketCommand = $"{couchbaseCli} bucket-create -c 127.0.0.1:8091 --username {this.Username} --password {this.Password} --bucket={bucket} --bucket-type couchbase --bucket-ramsize {memory} --enable-flush 1 --bucket-replica 0";
      return await this.ExecAsync(new[] { "/bin/sh", "-c", createBucketCommand });
    }

    public async Task<long> FlushBucket(string bucket)
    {
      var flushBucketCommand = $"{couchbaseCli} bucket-flush -c 127.0.0.1:8091 --username {this.Username} --password {this.Password} --bucket={bucket} --force";
      return await this.ExecAsync(new[] { "/bin/sh", "-c", flushBucketCommand });
    }
  }
}
