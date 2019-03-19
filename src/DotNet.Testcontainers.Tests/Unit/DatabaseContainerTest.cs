namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Data.SqlClient;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models.Database;
  using MySql.Data.MySqlClient;
  using Npgsql;
  using Xunit;

  public class DatabaseContainerTest
  {
    [Fact]
    public async Task MsSqlContainer()
    {
      // Given
      // When
      var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration
        {
          Password = "yourStrong(!)Password", // See following password policy: https://hub.docker.com/r/microsoft/mssql-server-linux/
        });

      // Then
      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        using (var connection = new SqlConnection(testcontainer.ConnectionString))
        {
          connection.Open();

          using (var cmd = new SqlCommand())
          {
            cmd.Connection = connection;
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteReader();
          }
        }
      }
    }

    [Fact]
    public async Task MySqlContainer()
    {
      // Given
      // When
      var testcontainersBuilder = new TestcontainersBuilder<MySqlTestcontainer>()
        .WithDatabase(new MySqlTestcontainerConfiguration
        {
          Database = "db",
          Username = "mysql",
          Password = "mysql",
        });

      // Then
      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        using (var connection = new MySqlConnection(testcontainer.ConnectionString))
        {
          connection.Open();

          using (var cmd = new MySqlCommand())
          {
            cmd.Connection = connection;
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteReader();
          }
        }
      }
    }

    [Fact]
    public async Task PostgreSqlContainer()
    {
      // Given
      // When
      var testcontainersBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
          Database = "db",
          Username = "postgres",
          Password = "postgres",
        });

      // Then
      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

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
      }
    }
  }
}
