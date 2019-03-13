namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models;
  using Npgsql;
  using Xunit;

  public class DatabaseContainerTest
  {
    [Fact]
    public async Task PostgreSqlContainer()
    {
      var testcontainersBuilder = new TestcontainersBuilder<PostgreSqlContainer>()
        .WithDatabase(new DatabaseConfiguration
        {
          Database = "db",
          Username = "postgres",
          Password = "postgres",
        });

      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        /*
        using (var connection = new NpgsqlConnection(testcontainer.ConnectionString))
        {
          connection.Open();

          using (var cmd = new NpgsqlCommand())
          {
            cmd.Connection = connection;
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteReader();
          }
        }
        */
      }
    }
  }
}
