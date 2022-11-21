namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Microsoft.Azure.Cosmos;
  using Xunit;

  public static class CosmosDbTestcontainerTest
  {
    public sealed class ConnectionTests : IClassFixture<CosmosDbFixture>, IDisposable
    {
      private const string SkipReason = "The Cosmos DB Linux Emulator Docker image does not run on every CI environment."; // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.

      private readonly HttpClient httpClient;

      private readonly CosmosClient cosmosClient;

      public ConnectionTests(CosmosDbFixture cosmosDbFixture)
        : this(cosmosDbFixture.Container.HttpClient, cosmosDbFixture.Container.ConnectionString)
      {
      }

      private ConnectionTests(HttpClient httpClient, string connectionString)
      {
        var cosmosClientOptions = new CosmosClientOptions();
        cosmosClientOptions.ConnectionMode = ConnectionMode.Gateway;
        cosmosClientOptions.HttpClientFactory = () => httpClient;

        this.httpClient = httpClient;
        this.cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);
      }

#pragma warning disable xUnit1004

      [Fact(Skip = SkipReason)]
      public async Task ConnectionEstablished()
      {
        var accountProperties = await this.cosmosClient.ReadAccountAsync()
          .ConfigureAwait(false);

        Assert.Equal("localhost", accountProperties.Id);
      }

      [Fact(Skip = SkipReason)]
      public async Task CreateDatabaseTest()
      {
        var databaseResponse = await this.cosmosClient.CreateDatabaseIfNotExistsAsync("db")
          .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.Created, databaseResponse.StatusCode);
      }

      [Fact(Skip = SkipReason)]
      public void CannotSetPassword()
      {
        var cosmosDb = new CosmosDbTestcontainerConfiguration();
        Assert.Throws<NotImplementedException>(() => cosmosDb.Password = string.Empty);
      }

#pragma warning restore xUnit1004

      public void Dispose()
      {
        this.cosmosClient.Dispose();
        this.httpClient.Dispose();
      }
    }
  }
}
