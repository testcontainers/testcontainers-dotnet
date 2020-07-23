namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using Couchbase;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Services;
  using DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.Databases;
  using Microsoft.Extensions.Logging;
  using Xunit;

  public class CouchbaseTestcontainerTest : IClassFixture<CouchbaseFixture>
  {
    private readonly CouchbaseFixture couchbaseFixture;

    public CouchbaseTestcontainerTest(CouchbaseFixture couchbaseFixture)
    {
      this.couchbaseFixture = couchbaseFixture;
    }

    [Fact]
    public async Task ConnectionEstablishedWithCrudTest()
    {
      // Given
      var customer1 = new Customer { Name = "Mustafa", Age = 29 };

      var customer2 = new Customer { Name = "Onur", Age = 30 };

      await using (var cluster = await Cluster.ConnectAsync(
        this.couchbaseFixture.Container.ConnectionString,
        this.couchbaseFixture.Container.Username,
        this.couchbaseFixture.Container.Password))
      {
        await using (var bucket = await cluster.BucketAsync("Customer"))
        {
          // When
          // Then
          var collection = bucket.DefaultCollection();

          // Create
          await collection.InsertAsync("customer-id-1", customer1);

          // Read
          var customer = (await collection.GetAsync("customer-id-1")).ContentAs<Customer>();
          Assert.Equal(customer1.Name, customer.Name);
          Assert.Equal(customer1.Age, customer.Age);

          // Update
          await collection.UpsertAsync("customer-id-1", customer2);

          customer = (await collection.GetAsync("customer-id-1")).ContentAs<Customer>();
          Assert.Equal(customer2.Name, customer.Name);
          Assert.Equal(customer2.Age, customer.Age);

          // Delete
          await collection.RemoveAsync("customer-id-1");
          Assert.False((await collection.ExistsAsync("customer-id-1")).Exists);
        }
      }
    }

    [Fact]
    public async Task ShouldCreateBucket()
    {
      // Given
      const string bucketName = nameof(this.ShouldCreateBucket);

      // When
      var exitCode = await this.couchbaseFixture.Container.CreateBucket(bucketName);

      await using (var cluster = await Cluster.ConnectAsync(
        this.couchbaseFixture.Container.ConnectionString,
        this.couchbaseFixture.Container.Username,
        this.couchbaseFixture.Container.Password))
      {
        var buckets = await cluster.Buckets.GetAllBucketsAsync();

        // Then
        Assert.True(buckets.ContainsKey(bucketName));
        Assert.Equal(0, exitCode);
      }
    }

    [Fact]
    public async Task ShouldFlushBucket()
    {
      // Given
      const string bucketName = nameof(this.ShouldFlushBucket);

      await this.couchbaseFixture.Container.CreateBucket(bucketName);

      await using (var cluster = await Cluster.ConnectAsync(
        this.couchbaseFixture.Container.ConnectionString,
        this.couchbaseFixture.Container.Username,
        this.couchbaseFixture.Container.Password))
      {
        var buckets = await cluster.Buckets.GetAllBucketsAsync();

        await using (var bucket = await cluster.BucketAsync(bucketName))
        {
          var collection = bucket.DefaultCollection();

          await cluster.QueryAsync<long>($"CREATE PRIMARY INDEX `#primary` ON `{bucketName}`");

          await collection.InsertAsync("1", new { Name = ".NET Testcontainers" });

          // When
          var exitCode = await this.couchbaseFixture.Container.FlushBucket(bucketName);

          var result = await cluster.QueryAsync<dynamic>($"SELECT * FROM {bucketName}");
          var rows = await result.Rows.ToListAsync();

          // Then
          Assert.True(buckets.ContainsKey(bucketName));
          Assert.Equal(0, exitCode);
          Assert.Empty(rows);
        }
      }
    }

    [Fact]
    public void ShouldNotThrowArgumentOutOfRangeExceptionWhenClusterConfigurationIsValid()
    {
      // Given
      CouchbaseTestcontainerConfiguration configuration = null;

      Action action = () => configuration = new CouchbaseTestcontainerConfiguration
      {
        BucketName = "Bucket",
        BucketType = "MEMCACHED",
        ClusterRamSize = "1024",
        ClusterIndexRamSize = "512",
        ClusterEventingRamSize = "256",
        ClusterFtsRamSize = "256",
        ClusterAnalyticsRamSize = "1024"
      };

      // When
      var exception = Record.Exception(action.Invoke);

      // Then
      Assert.Null(exception);
      Assert.Equal("Bucket", configuration.BucketName);
      Assert.Equal("MEMCACHED", configuration.BucketType);
      Assert.Equal("1024", configuration.ClusterRamSize);
      Assert.Equal("512", configuration.ClusterIndexRamSize);
      Assert.Equal("256", configuration.ClusterEventingRamSize);
      Assert.Equal("256", configuration.ClusterFtsRamSize);
      Assert.Equal("1024", configuration.ClusterAnalyticsRamSize);
    }

    [Fact]
    public void ShouldThrowArgumentExceptionWhenRamSizeIsNotAnInteger()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterRamSize = nameof(double.NaN) };

      // When
      var exception = Assert.Throws<ArgumentException>(action.Invoke);

      // Then
      Assert.Equal("NaN is not an integer. (Parameter 'ClusterRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterRamSize = "1023" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterRamSize ram size can not be less than 1024 MB. (Parameter 'ClusterRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterAnalyticsRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterAnalyticsRamSize = "1023" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterAnalyticsRamSize ram size can not be less than 1024 MB. (Parameter 'ClusterAnalyticsRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterEventingRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterEventingRamSize = "255" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterEventingRamSize ram size can not be less than 256 MB. (Parameter 'ClusterEventingRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterFtsRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterFtsRamSize = "255" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterFtsRamSize ram size can not be less than 256 MB. (Parameter 'ClusterFtsRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterIndexRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterIndexRamSize = "511" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterIndexRamSize ram size can not be less than 512 MB. (Parameter 'ClusterIndexRamSize')", exception.Message);
    }

    private sealed class Customer
    {
      public string Name { get; set; }

      public int Age { get; set; }
    }
  }
}
