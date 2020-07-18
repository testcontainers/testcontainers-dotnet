namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Linq;
  using System.Threading.Tasks;
  using Couchbase;
  using Fixtures;
  using Xunit;

  public class CouchbaseTestcontainerTest : IClassFixture<Couchbase>
  {

    private readonly Couchbase couchbase;

    public CouchbaseTestcontainerTest(Couchbase couchbase)
    {
      this.couchbase = couchbase;
    }

    /// <summary>
    /// Sample Couchbase document class for test
    /// </summary>
    private class Customer
    {
      public string Name { get; set; }
      public int Age{ get; set; }
    }

    [Fact]
    public async Task ConnectionEstablished_WithCrudTest()
    {
      //Given
      var cluster = await Cluster.ConnectAsync(this.couchbase.Container.ConnectionString, "Administrator", "password");
      var bucket = await cluster.BucketAsync("Customer");
      var collection = bucket.DefaultCollection();
      //When & Then

      //Create
      await collection.InsertAsync(
        "customer-id-1",
        new Customer {Name = "Mustafa", Age = 29}
      );

      //Read
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
    }

    [Fact]
    public async Task CreateNewBucketTest()
    {
      //Given
      await this.couchbase.Container.CreateNewBucket("dotnet-testcontainers");

      //When
      var cluster = await Cluster.ConnectAsync(this.couchbase.Container.ConnectionString, "Administrator", "password");
      var bucket = await cluster.BucketAsync("dotnet-testcontainers");

      //Then
      Assert.NotNull(await bucket.PingAsync());
    }

    [Fact]
    public async Task FlushBucketTest()
    {
      //Given
      var cluster = await Cluster.ConnectAsync(this.couchbase.Container.ConnectionString, "Administrator", "password");
      await cluster.QueryAsync<long>("CREATE PRIMARY INDEX `#primary` ON `Customer`");
      var bucket = await cluster.BucketAsync("Customer");
      var collection = bucket.DefaultCollection();

      await collection.InsertAsync(
        "customer-id-2",
        new Customer {Name = "Ozan", Age = 26}
      );

      //When
      await this.couchbase.Container.FlushBucket("Customer");

      //Then
      var queryResult = await cluster.QueryAsync<long>("SELECT RAW count(meta().id) FROM Customer");

      var count = queryResult.Rows.ToListAsync().Result.First();
      Assert.Equal(0,count);
    }

  }

}
