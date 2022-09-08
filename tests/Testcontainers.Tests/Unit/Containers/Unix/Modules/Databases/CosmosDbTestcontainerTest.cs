namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Microsoft.Azure.Cosmos;
  using Xunit;

  public static class CosmosDbTestcontainerTest
  {
   [Collection(nameof(Testcontainers))]
   public sealed class ConnectionTests : IClassFixture<CosmosDbFixture>
   {
      private static readonly CosmosClientOptions SkipSslValidationOptions = new CosmosClientOptions()
      {
         HttpClientFactory = () =>
         {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler()
            {
               ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };

            return new HttpClient(httpMessageHandler);
         },

         ConnectionMode = ConnectionMode.Gateway,
      };

      private readonly CosmosDbFixture commonContainerPorts;

      public ConnectionTests(CosmosDbFixture commonContainerPorts)
      {
         this.commonContainerPorts = commonContainerPorts;
      }

      [Fact]
      public async Task ShouldEstablishConnection()
      {
         var exception = await Record.ExceptionAsync(() => EstablishConnection(this.commonContainerPorts));
         Assert.Null(exception);
      }

      private static async Task EstablishConnection(CosmosDbFixture cosmosDb)
      {
         var client = new CosmosClient(cosmosDb.Container.ConnectionString, SkipSslValidationOptions);

         var properties = await client.ReadAccountAsync()
            .ConfigureAwait(false);
      }
    }
  }
}
