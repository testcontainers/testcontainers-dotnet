
Here is an example of a pre-configured container. In the example,  Testcontainers starts a PostgreSQL database in a [xUnit.net][xunit] test and executes a SQL query.

```csharp
public sealed class PostgreSqlTest : IAsyncLifetime
{
  private readonly TestcontainerDatabase testcontainers = new TestcontainersBuilder<PostgreSqlTestcontainer>()
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
    using (var connection = new NpgsqlConnection(this.testcontainers.ConnectionString))
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
    return this.testcontainers.StartAsync();
  }
  public Task DisposeAsync()
  {
    return this.testcontainers.DisposeAsync().AsTask();
  }
}
```

The implementation of the pre-configured wait strategies can be chained together to support individual requirements for Testcontainers with different container platform operating systems.

```csharp
Wait.ForUnixContainer()
  .UntilPortIsAvailable(80)
  .UntilFileExists("/tmp/foo")
  .UntilFileExists("/tmp/bar")
  .UntilOperationIsSucceeded(() => true, 1);
```
