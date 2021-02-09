namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.Databases
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Xunit;

  public class CouchbaseFixture : ModuleFixture<CouchbaseTestcontainer>, IAsyncLifetime
  {
    public CouchbaseFixture()
      : base(new TestcontainersBuilder<CouchbaseTestcontainer>()
        .WithDatabase(new CouchbaseTestcontainerConfiguration
        {
          Username = "Administrator",
          Password = "password",
          BucketName = "customers",
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

    public Task InitializeAsync()
    {
      // See comments in: CouchbaseTestcontainerTest.
      return Task.CompletedTask;
      return this.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
      // See comments in: CouchbaseTestcontainerTest.
      return Task.CompletedTask;
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
