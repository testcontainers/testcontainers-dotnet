namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.Databases
{
  using System.Threading.Tasks;
  using Couchbase;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Xunit;

  public class CouchbaseFixture : ModuleFixture<CouchbaseTestcontainer>, IAsyncLifetime
  {
    public const string BucketName = "Sample";

    private const string Username = "Administrator";

    private const string Password = "password";

    public CouchbaseFixture()
      : base(new TestcontainersBuilder<CouchbaseTestcontainer>()
        .WithDatabase(new CouchbaseTestcontainerConfiguration
        {
          Username = Username,
          Password = Password,
          BucketName = BucketName,
          ClusterRamSize = "384",
          ClusterIndexRamSize = "256",
          ClusterFtsRamSize = "256",
          ClusterEventingRamSize = "256",
          ClusterAnalyticsRamSize = "1024"
        })
        // Required ports for client to node communication: https://docs.couchbase.com/server/current/install/install-ports.html#detailed-port-description.
        .WithPortBinding(8091)
        .WithPortBinding(8092)
        .WithPortBinding(8093)
        .WithPortBinding(8094)
        .WithPortBinding(11210)
        .Build())
    {
    }

    public ICluster Cluster { get; private set; }

    public async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Cluster = await Couchbase.Cluster.ConnectAsync(
          this.Container.ConnectionString,
          this.Container.Username,
          this.Container.Password)
        .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
      await this.Cluster.DisposeAsync()
        .AsTask()
        .ConfigureAwait(false);

      await this.Container.DisposeAsync()
        .AsTask()
        .ConfigureAwait(false);
    }
  }
}
