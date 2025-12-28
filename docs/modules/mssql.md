# Microsoft SQL Server

[Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server), also known as MSSQL, is a relational database engine developed by Microsoft and is a popular choice in enterprise systems.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.MsSql
```

You can start a MSSQL container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a MSSQL container"
    ```csharp
    var msSqlContainer = new MsSqlBuilder().Build();
    await msSqlContainer.StartAsync();
    ```

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.MsSql.Tests/MsSqlContainerTest.cs:UseMsSqlContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.MsSql.Tests/Testcontainers.MsSql.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## A Note To Developers

Once Testcontainers creates a server instance, developers may use the connection string with any of the popular data-access technologies found in the .NET ecosystem. Some of these libraries include [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore), [Dapper](https://www.nuget.org/packages/Dapper), and [NHibernate](https://www.nuget.org/packages/NHibernate). At which point, developers can execute database migrations and SQL scripts.