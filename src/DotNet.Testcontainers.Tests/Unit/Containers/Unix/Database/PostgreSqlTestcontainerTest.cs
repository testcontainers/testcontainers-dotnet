namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Npgsql;
  using Xunit;

  public class PostgreSqlTestcontainerTest
  {
    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
          Database = "db",
          Username = "postgres",
          Password = "postgres",
        });

      // When
      // Then
      await using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        await using (var connection = new NpgsqlConnection(testcontainer.ConnectionString))
        {
          connection.Open();

          await using (var cmd = new NpgsqlCommand())
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
