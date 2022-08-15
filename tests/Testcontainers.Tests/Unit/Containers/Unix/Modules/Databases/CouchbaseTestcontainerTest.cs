namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using Couchbase.KeyValue;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class CouchbaseTestcontainerTest : IClassFixture<CouchbaseFixture>
  {
    private readonly CouchbaseFixture couchbaseFixture;

    public CouchbaseTestcontainerTest(CouchbaseFixture couchbaseFixture)
    {
      this.couchbaseFixture = couchbaseFixture;
    }

    [Fact]
    public async Task ConnectToExistingBucketAndRunCrudOperations()
    {
      // Given
      var cluster = this.couchbaseFixture.Connection;

      var id = Guid.NewGuid().ToString();

      var customer1 = new Customer("Mustafa", 29);

      var customer2 = new Customer("Onur", 30);

      // When
      await using var bucket = await cluster.BucketAsync("Sample")
        .ConfigureAwait(false);

      // Then
      var collection = await bucket.DefaultCollectionAsync()
        .ConfigureAwait(false);

      // Create
      _ = await collection.InsertAsync(id, customer1)
        .ConfigureAwait(false);

      var createResult = (await collection.GetAsync(id)
          .ConfigureAwait(false))
        .ContentAs<Customer>();

      Assert.Equal(customer1.Name, createResult.Name);
      Assert.Equal(customer1.Age, createResult.Age);

      // Update
      _ = await collection.UpsertAsync(id, customer2)
        .ConfigureAwait(false);

      var updateResult = (await collection.GetAsync(id)
          .ConfigureAwait(false))
        .ContentAs<Customer>();

      Assert.Equal(customer2.Name, updateResult.Name);
      Assert.Equal(customer2.Age, updateResult.Age);

      // Delete
      await collection.RemoveAsync(id)
        .ConfigureAwait(false);

      Assert.False((await collection.ExistsAsync(id, o => o.Timeout(TimeSpan.FromMinutes(1)))
        .ConfigureAwait(false)).Exists);
    }

    [Fact]
    public async Task CreateBucket()
    {
      // Given
      var cluster = this.couchbaseFixture.Connection;

      var bucketName = Guid.NewGuid().ToString();

      // When
      _ = await this.couchbaseFixture.Container.CreateBucket(bucketName)
        .ConfigureAwait(false);

      await using var bucket = await cluster.BucketAsync(bucketName)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(bucketName, bucket.Name);
    }

    [Fact]
    public async Task FlushBucket()
    {
      // Given
      var cluster = this.couchbaseFixture.Connection;

      var id = Guid.NewGuid().ToString();

      var bucketName = Guid.NewGuid().ToString();

      _ = await this.couchbaseFixture.Container.CreateBucket(bucketName)
        .ConfigureAwait(false);

      // When
      await using var bucket = await cluster.BucketAsync(bucketName)
        .ConfigureAwait(false);

      // Then
      var collection = await bucket.DefaultCollectionAsync()
        .ConfigureAwait(false);

      // Create
      _ = await collection.InsertAsync(id, new { }, o => o.Timeout(TimeSpan.FromMinutes(1)))
        .ConfigureAwait(false);

      Assert.True((await collection.ExistsAsync(id, o => o.Timeout(TimeSpan.FromMinutes(1)))
        .ConfigureAwait(false)).Exists);

      // Flush
      _ = await this.couchbaseFixture.Container.FlushBucket(bucketName)
        .ConfigureAwait(false);

      Assert.False((await collection.ExistsAsync(id, o => o.Timeout(TimeSpan.FromMinutes(1)))
        .ConfigureAwait(false)).Exists);
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
        BucketRamSize = "1024",
        ClusterRamSize = "256",
        ClusterIndexRamSize = "256",
        ClusterFtsRamSize = "256",
        ClusterEventingRamSize = "256",
        ClusterAnalyticsRamSize = "1024",
      };

      // When
      var exception = Record.Exception(action.Invoke);

      // Then
      Assert.Null(exception);
      Assert.Equal("Bucket", configuration.BucketName);
      Assert.Equal("MEMCACHED", configuration.BucketType);
      Assert.Equal("1024", configuration.BucketRamSize);
      Assert.Equal("256", configuration.ClusterRamSize);
      Assert.Equal("256", configuration.ClusterIndexRamSize);
      Assert.Equal("256", configuration.ClusterFtsRamSize);
      Assert.Equal("256", configuration.ClusterEventingRamSize);
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
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterRamSize = "255" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterRamSize ram size can not be less than 256 MB. (Parameter 'ClusterRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterIndexRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration { ClusterIndexRamSize = "255" };

      // When
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);

      // Then
      Assert.Equal("Couchbase ClusterIndexRamSize ram size can not be less than 256 MB. (Parameter 'ClusterIndexRamSize')", exception.Message);
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
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      const string script = @"
        CREATE PRIMARY INDEX ON `MyBucket`;
        SELECT * FROM system:indexes;
      ";

      // When
      _ = await this.couchbaseFixture.Container.CreateBucket("MyBucket")
        .ConfigureAwait(false);

      var result = await this.couchbaseFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(0, result.ExitCode);
      Assert.Contains("MyBucket", result.Stdout);
    }

    private readonly struct Customer
    {
      public Customer(string name, int age)
      {
        this.Name = name;
        this.Age = age;
      }

      public string Name { get; }

      public int Age { get; }
    }
  }
}
