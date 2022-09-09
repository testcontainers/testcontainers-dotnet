namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Microsoft.Azure.Cosmos;
  using Xunit;

  public static class CosmosDbTestcontainerTest
  {
    [Collection(nameof(Testcontainers))]
    public sealed class ConnectionTests : IClassFixture<CosmosDbFixture>
    {
      private readonly CosmosDbFixture fixture;

      public ConnectionTests(CosmosDbFixture fixture)
      {
        this.fixture = fixture;
      }

      private CosmosClientOptions Options => new CosmosClientOptions
      {
        HttpClientFactory = () => this.fixture.Container.HttpClient,
        ConnectionMode = ConnectionMode.Gateway,
      };

      [Fact]
      public async Task ShouldEstablishConnection()
      {
        var client = new CosmosClient(this.fixture.Container.ConnectionString, this.Options);

        var accountProperties = await client.ReadAccountAsync();
        Assert.Equal("localhost", accountProperties.Id);
      }

      [Fact]
      public async Task CreateDatabaseTest()
      {
        var client = new CosmosClient(this.fixture.Container.ConnectionString, this.Options);

        var db = await client.CreateDatabaseIfNotExistsAsync("test-db");
        Assert.Equal(HttpStatusCode.Created, db.StatusCode);
      }
    }
  }
}

