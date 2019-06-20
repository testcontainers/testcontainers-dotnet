namespace DotNet.Testcontainers.Tests.Unit.Linux.Database
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models.Database;
  using MyCouch;
  using Xunit;

  public class CouchDbTestcontainerTest
  {
    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<CouchDbTestcontainer>()
        .WithDatabase(new CouchDbTestcontainerConfiguration
        {
          Database = "db",
          Username = "couchdb",
          Password = "couchdb",
        });

      // When
      // Then
      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        using (var connection = new MyCouchClient(testcontainer.ConnectionString, testcontainer.Database))
        {
          await connection.Documents.PostAsync("{\"name\":\".NET Testcontainers\"}");
        }
      }
    }
  }
}
