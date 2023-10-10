# Microsoft SQL Server

[Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server), also known as MSSQL, is a relational database engine developed by Microsoft and is a popular choice in enterprise systems. The following example provides .NET developers with a starting point to use a Microsoft SQL Server instance in the [xUnit][xunit] tests.

The following example uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.MsSql
dotnet add package Microsoft.Data.SqlClient
dotnet add package xunit
```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace TestcontainersModules;

public sealed class MsSqlServerContainerTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer
        = new MsSqlBuilder().Build();

    [Fact]
    public async Task ReadFromMsSqlDatabase()
    {
        await using var connection = new SqlConnection(_msSqlContainer.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1;";

        var actual = await command.ExecuteScalarAsync() as int?;
        Assert.Equal(1, actual.GetValueOrDefault());
    }

    public Task InitializeAsync()
        => _msSqlContainer.StartAsync();

    public Task DisposeAsync()
        => _msSqlContainer.DisposeAsync().AsTask();
}
```

To execute the tests, use the command `dotnet test` from a terminal.

## A Note To Developers

Once Testcontainers creates a server instance, developers may use the connection string with any of the popular data-access technologies found in the .NET Ecosystem. Some of these libraries include [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore), [Dapper](https://www.nuget.org/packages/Dapper), and [NHibernate](https://www.nuget.org/packages/NHibernate). At which point, developers can execute database migrations and SQL scripts.

[xunit]: https://xunit.net/
