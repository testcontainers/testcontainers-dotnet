# PostgreSQL

Here is an example of a pre-configured PostgreSQL container. In the example, Testcontainers starts a PostgreSQL database in a [xUnit.net][xunit] test and executes a SQL query against it.

```csharp
public sealed class PostgreSqlTest : IAsyncLifetime
{
  private readonly TestcontainerDatabase _postgresqlContainer = new ContainerBuilder<PostgreSqlTestcontainer>()
    .WithDatabase(new PostgreSqlTestcontainerConfiguration
    {
      Database = "db",
      Username = "postgres",
      Password = "postgres",
    })
    .Build();

  [Fact]
  public void ExecuteCommand()
  {
    using (var connection = new NpgsqlConnection(_postgresqlContainer.ConnectionString))
    {
      using (var command = new NpgsqlCommand())
      {
        connection.Open();
        command.Connection = connection;
        command.CommandText = "SELECT 1";
        command.ExecuteReader();
      }
    }
  }

  public Task InitializeAsync()
  {
    return _postgresqlContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _postgresqlContainer.DisposeAsync().AsTask();
  }
}
```

[xunit]: https://xunit.net/
