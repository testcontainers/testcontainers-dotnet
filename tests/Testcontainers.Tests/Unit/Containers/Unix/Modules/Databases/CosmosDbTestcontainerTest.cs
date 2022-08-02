namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Tests.Fixtures;
  using System.Threading.Tasks;
  using Xunit;

  public class CosmosDbTestcontainerTest : IClassFixture<CosmosDbFixture>
    {
       private readonly CosmosDbFixture cosmosDbFixture;

       public CosmosDbTestcontainerTest(CosmosDbFixture cosmosDbFixture)
       {
          this.cosmosDbFixture = cosmosDbFixture;
       }

       [Fact]
       public async Task DatabaseCreated()
       {
            var dbResponse = await this.cosmosDbFixture.Container.CreateDatabaseAsync()
                .ConfigureAwait(false);

          Assert.Equal(System.Net.HttpStatusCode.Created, dbResponse.StatusCode);
       }
    }
}
