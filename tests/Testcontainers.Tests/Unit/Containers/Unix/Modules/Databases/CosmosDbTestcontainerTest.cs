namespace DotNet.Testcontainers.Tests.Unit
{
  using Microsoft.Azure.Cosmos;
  using DotNet.Testcontainers.Tests.Fixtures;
  using System.Threading.Tasks;
  using Xunit;
  using System.Net.Http;
  using System.Data.Common;
  using DotNet.Testcontainers.Containers;

  public static class CosmosDbTestcontainerTest
  {
   [Collection(nameof(Testcontainers))]
   public sealed class SqlApi : IClassFixture<CosmosDbFixture.CosmosDbDefaultFixture>
   {
      private static readonly CosmosClientOptions skipSslValidationOptions = new CosmosClientOptions()
      {
         HttpClientFactory = () =>
         {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler()
            {
               ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return new HttpClient(httpMessageHandler);
         },

         ConnectionMode = ConnectionMode.Gateway
      };

      private readonly CosmosDbFixture.CosmosDbDefaultFixture commonContainerPorts;

      private CosmosDbTestcontainer Container;

      public SqlApi(CosmosDbFixture.CosmosDbDefaultFixture commonContainerPorts)
      {
         this.commonContainerPorts = commonContainerPorts;
      }

      [Fact]
      public async Task ShouldEstablishConnection()
      {
         var exception = await Record.ExceptionAsync(() => Task.WhenAll(EstablishConnection(commonContainerPorts)));
         Assert.Null(exception);
      }

      private static async Task EstablishConnection(CosmosDbFixture.CosmosDbDefaultFixture cosmosDb)
      {
         var client = new CosmosClient(cosmosDb.Container.ConnectionString, skipSslValidationOptions);

         var properties = await client.ReadAccountAsync()
            .ConfigureAwait(false);

         Assert.Equal(cosmosDb.Configuration.SqlApiPort, cosmosDb.Container.SqlApiPort);
      }
    }
  }
}
