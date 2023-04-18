# PostgreSQL

Here is an example of a pre-configured PostgreSQL [module](https://www.nuget.org/packages/Testcontainers.PostgreSql). In the example, Testcontainers starts a PostgreSQL database in a [xUnit.net][xunit] test and executes a SQL query against it.

```csharp
public sealed class PostgreSqlContainerTest : IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

  public Task InitializeAsync()
  {
    return _postgreSqlContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _postgreSqlContainer.DisposeAsync().AsTask();
  }

  [Fact]
  public void ExecuteCommand()
  {
    using (DbConnection connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString()))
    {
      using (DbCommand command = new NpgsqlCommand())
      {
        connection.Open();
        command.Connection = connection;
        command.CommandText = "SELECT 1";
      }
    }
  }
}
```

[xunit]: https://xunit.net/
