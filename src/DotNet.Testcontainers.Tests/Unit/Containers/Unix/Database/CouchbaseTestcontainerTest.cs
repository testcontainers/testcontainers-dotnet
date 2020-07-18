namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Couchbase;
  using Testcontainers.Containers.Builders;
  using Testcontainers.Containers.Configurations.Databases;
  using Testcontainers.Containers.Modules.Databases;
  using Testcontainers.Containers.OutputConsumers;
  using Testcontainers.Containers.WaitStrategies;
  using Xunit;
  using Xunit.Abstractions;

  public class CouchbaseTestcontainerTest
  {
    private readonly ITestOutputHelper testOutputHelper;

    public CouchbaseTestcontainerTest(ITestOutputHelper testOutputHelper)
    {
      this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task ConnectionEstablished_WithCrudExample()
    {
      //Given
      var OutStream = new MemoryStream();
      var ErrorStream = new MemoryStream();
      var outputConsumer = Consume.RedirectStdoutAndStderrToStream(OutStream, ErrorStream);
      var couchbaseTestcontainer = new TestcontainersBuilder<CouchbaseTestcontainer>()
        .WithDatabase(new CouchbaseTestcontainerConfiguration("Administrator", "password", "CRUD"))
        .WithOutputConsumer(outputConsumer)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(outputConsumer.Stdout, "couchbase-dev started"))
        .WithExposedPort(8091)
        .WithExposedPort(11210)
        .WithPortBinding(8091, 8091)
        .WithPortBinding(11210, 11210)
        .Build();

      await couchbaseTestcontainer.StartAsync();

      //When & Then
      var cluster = await Cluster.ConnectAsync(couchbaseTestcontainer.ConnectionString, "Administrator","password");
      var bucket = await cluster.BucketAsync("CRUD");
      var collection = bucket.DefaultCollection();

      // Create
      await collection.InsertAsync(
        "customer-id-1",
        new Customer {Name = "Mustafa", Age = 29}
        );

      // Read
      var customer = (await collection.GetAsync("customer-id-1")).ContentAs<Customer>();
      Assert.True(customer.Name == "Mustafa");
      Assert.True(customer.Age == 29);

      //Update
      customer.Age = 30;
      customer.Name = "Onur";
      await collection.UpsertAsync("customer-id-1", customer);

      var updatedCustomer = (await collection.GetAsync("customer-id-1")).ContentAs<Customer>();
      Assert.True(updatedCustomer.Name == "Onur");
      Assert.True(updatedCustomer.Age == 30);

      //Delete
      await collection.RemoveAsync("customer-id-1");
      var existsResult = await collection.ExistsAsync("customer-id-1");
      Assert.False(existsResult.Exists);
      await couchbaseTestcontainer.StopAsync();
    }

    private class Customer
    {
      public string Name { get; set; }
      public int Age{ get; set; }
    }

    [Fact]
    public void ShouldNotThrowsArgumentOutOfRangeException_WhenClusterConfigurationIsValid()
    {
      //Given
      var configuration = new CouchbaseTestcontainerConfiguration("Administrator", "password", "dotnet-testcontainers");

      //When & Then
      Func<CouchbaseTestcontainerConfiguration> action = () => configuration
        .ClusterRamSize(1024)
        .ClusterIndexRamSize(512)
        .ClusterEventingRamSize(256)
        .ClusterFtsRamSize(256)
        .ClusterAnalyticsRamSize(1024);;
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterRamSizeIsNotValid()
    {
      //Given
      var configuration = new CouchbaseTestcontainerConfiguration("Administrator", "password", "test");

      //When
      Func<CouchbaseTestcontainerConfiguration> action = () => configuration.ClusterRamSize(1023);

      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.True(exception.Message == "Couchbase clusterRamSize ram size can not be less than 1024 MB. (Parameter 'clusterRamSize')");
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterAnalyticsRamSizeIsNotValid()
    {
      //Given
      var configuration = new CouchbaseTestcontainerConfiguration("Administrator", "password", "test");

      //When
      Func<CouchbaseTestcontainerConfiguration> action = () => configuration.ClusterAnalyticsRamSize(1023);

      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.True(exception.Message == "Couchbase clusterAnalyticsRamSize ram size can not be less than 1024 MB. (Parameter 'clusterAnalyticsRamSize')");
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterEventingRamSizeIsNotValid()
    {
      //Given
      var configuration = new CouchbaseTestcontainerConfiguration("Administrator", "password", "test");

      //When
      Func<CouchbaseTestcontainerConfiguration> action = () => configuration.ClusterEventingRamSize(255);

      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.True(exception.Message == "Couchbase clusterEventingRamSize ram size can not be less than 256 MB. (Parameter 'clusterEventingRamSize')");
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterFtsRamSizeIsNotValid()
    {
      //Given
      var configuration = new CouchbaseTestcontainerConfiguration("Administrator", "password", "test");

      //When
      Func<CouchbaseTestcontainerConfiguration> action = () => configuration.ClusterFtsRamSize(255);

      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.True(exception.Message == "Couchbase clusterFtsRamSize ram size can not be less than 256 MB. (Parameter 'clusterFtsRamSize')");
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterIndexRamSizeIsNotValid()
    {
      //Given
      var configuration = new CouchbaseTestcontainerConfiguration("Administrator", "password", "test");

      //When
      Func<CouchbaseTestcontainerConfiguration> action = () => configuration.ClusterIndexRamSize(511);

      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.True(exception.Message == "Couchbase clusterIndexRamSize ram size can not be less than 512 MB. (Parameter 'clusterIndexRamSize')");
    }

  }

}
