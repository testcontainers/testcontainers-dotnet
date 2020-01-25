namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using MySql.Data.MySqlClient;
  using Xunit;

  public class MySqlTestcontainerTest
  {
    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<MySqlTestcontainer>()
        .WithDatabase(new MySqlTestcontainerConfiguration
        {
          Database = "db",
          Username = "mysql",
          Password = "mysql"
        });

      // When
      // Then
      await using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        await using (var connection = new MySqlConnection(testcontainer.ConnectionString))
        {
          connection.Open();

          await using (var cmd = new MySqlCommand())
          {
            cmd.Connection = connection;
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteReader();
          }
        }
      }
    }
  }
}
