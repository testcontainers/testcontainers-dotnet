namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Linq;
  using System.Threading.Tasks;
  using Couchbase;
  using DotNet.Testcontainers.Tests.Fixtures.Containers.Modules.Databases;
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
    public async Task ShouldContainCreatedBucket()
    {
      // Given
      const string bucketName = nameof(this.ShouldContainCreatedBucket);

      await this.couchbaseFixture.Container.CreateNewBucket(bucketName);

      await using (var cluster = await Cluster.ConnectAsync(
        this.couchbaseFixture.Container.ConnectionString,
        this.couchbaseFixture.Container.Username,
        this.couchbaseFixture.Container.Password))
      {
        // When
        var buckets = await cluster.Buckets.GetAllBucketsAsync();

        // Then
        Assert.True(buckets.ContainsKey(bucketName));
      }
    }

    [Fact]
    public async Task ShouldNotContainFlushedBucket()
    {
      // Given
      const string bucketName = "Example";

      await this.couchbaseFixture.Container.CreateNewBucket(bucketName);

      await using (var cluster = await Cluster.ConnectAsync(
        this.couchbaseFixture.Container.ConnectionString,
        this.couchbaseFixture.Container.Username,
        this.couchbaseFixture.Container.Password))
      {
        await cluster.QueryAsync<long>($"CREATE PRIMARY INDEX `#primary` ON `{bucketName}`");

        await using (var bucket = await cluster.BucketAsync(bucketName))
        {
          var collection = bucket.DefaultCollection();

          await collection.InsertAsync("1", new { Phone = "123456" });

          // When
          await this.couchbaseFixture.Container.FlushBucket(bucketName);

          // Then
          var result = await cluster.QueryAsync<long>($"SELECT * FROM {bucketName}"); // Don't do this.
          Assert.Empty(await result.Rows.ToListAsync());
        }
      }
    }

    private class Customer
    {
      public string Name { get; set; }

      public int Age { get; set; }
    }
  }
}
