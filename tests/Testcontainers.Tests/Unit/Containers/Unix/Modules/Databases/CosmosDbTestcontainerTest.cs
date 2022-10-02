namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Microsoft.Azure.Cosmos;
  using Xunit;

  public static class CosmosDbTestcontainerTest
  {
    [Collection(nameof(Testcontainers))]
    public sealed class ConnectionTests : IClassFixture<CosmosDbFixture>, IDisposable
    {
      private readonly HttpClient httpClient;

      private readonly CosmosClient cosmosClient;

      public ConnectionTests(CosmosDbFixture cosmosDbFixture)
        : this(cosmosDbFixture.Container.HttpClient, cosmosDbFixture.Container.ConnectionString)
      {
      }

      private ConnectionTests(HttpClient httpClient, string connectionString)
      {
        var cosmosClientOptions = new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway, HttpClientFactory = () => httpClient };
        this.httpClient = httpClient;
        this.cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);
      }

      [Fact(Skip = "Waiting for a working cosmosdb emulator")]
      public async Task ShouldEstablishConnection()
      {
        var accountProperties = await this.cosmosClient.ReadAccountAsync()
          .ConfigureAwait(false);

        Assert.Equal("localhost", accountProperties.Id);
      }

      [Fact(Skip = "Waiting for a working cosmosdb emulator")]
      public async Task CreateDatabaseTest()
      {
        var databaseResponse = await this.cosmosClient.CreateDatabaseIfNotExistsAsync("db")
          .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.Created, databaseResponse.StatusCode);
      }

      public void Dispose()
      {
        this.cosmosClient.Dispose();
        this.httpClient.Dispose();
      }
    }
  }
}
