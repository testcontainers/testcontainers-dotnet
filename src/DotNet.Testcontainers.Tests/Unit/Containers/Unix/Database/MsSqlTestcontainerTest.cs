namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System;
  using System.Data.SqlClient;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Xunit;

  public class MsSqlTestcontainerTest
  {
    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration
        {
          Password = "yourStrong(!)Password", // See following password policy: https://hub.docker.com/r/microsoft/mssql-server-linux/
        });

      // When
      // Then
      await using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        await using (var connection = new SqlConnection(testcontainer.ConnectionString))
        {
          connection.Open();

          await using (var cmd = new SqlCommand())
          {
            cmd.Connection = connection;
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteReader();
          }
        }
      }
    }

    [Fact]
    public void CannotSetDatabase()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Username = string.Empty);
    }
  }
}
