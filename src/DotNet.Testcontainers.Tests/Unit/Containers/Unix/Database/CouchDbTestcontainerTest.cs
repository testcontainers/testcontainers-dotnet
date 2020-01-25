namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
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
      await using (var testcontainer = testcontainersBuilder.Build())
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
