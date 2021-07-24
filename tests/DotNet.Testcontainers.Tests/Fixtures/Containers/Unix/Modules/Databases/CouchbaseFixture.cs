namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using Couchbase;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  public sealed class CouchbaseFixture : ModuleFixture<CouchbaseTestcontainer>
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
          ClusterAnalyticsRamSize = "1024",
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

    public Task<ICluster> GetCluster()
    {
      return Cluster.ConnectAsync(this.Container.ConnectionString, this.Container.Username, this.Container.Password);
    }

    public override Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
