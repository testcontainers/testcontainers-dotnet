namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using Couchbase;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class CouchbaseFixture : DatabaseFixture<CouchbaseTestcontainer, ICluster>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new CouchbaseTestcontainerConfiguration
    {
      Username = "Administrator",
      Password = "password",
      BucketName = "Sample",
      ClusterRamSize = "384",
      ClusterIndexRamSize = "256",
      ClusterFtsRamSize = "256",
      ClusterEventingRamSize = "256",
      ClusterAnalyticsRamSize = "1024",
    };

    public CouchbaseFixture()
    {
      this.Container = new TestcontainersBuilder<CouchbaseTestcontainer>()
        .WithDatabase(this.configuration)

        // Required ports for client to node communication: https://docs.couchbase.com/server/current/install/install-ports.html#detailed-port-description.
        .WithPortBinding(8091)
        .WithPortBinding(8092)
        .WithPortBinding(8093)
        .WithPortBinding(8094)
        .WithPortBinding(11210)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = await Cluster.ConnectAsync(this.Container.ConnectionString, this.Container.Username, this.Container.Password)
        .ConfigureAwait(false);
    }

    public override async Task DisposeAsync()
    {
      await this.Connection.DisposeAsync()
        .ConfigureAwait(false);

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
