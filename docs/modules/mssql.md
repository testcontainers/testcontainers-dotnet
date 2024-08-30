# Microsoft SQL Server

[Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server), also known as MSSQL, is a relational database engine developed by Microsoft and is a popular choice in enterprise systems.

Add the following dependency to your project file:

```console title="NuGet"
dotnet add package Testcontainers.MsSql
```

You can start a MSSQL container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

<!--codeinclude-->
[Create Container Instance](../../tests/Testcontainers.MsSql.Tests/MsSqlContainerTest.cs) inside_block:CreateMsSqlContainer
<!--/codeinclude-->

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

<!--codeinclude-->
[Usage Example](../../tests/Testcontainers.MsSql.Tests/MsSqlContainerTest.cs) inside_block:UseMsSqlContainer
<!--/codeinclude-->

The test example uses the following NuGet dependencies:

<!--codeinclude-->
[Package References](../../tests/Testcontainers.MsSql.Tests/Testcontainers.MsSql.Tests.csproj) inside_block:PackageReferences
<!--/codeinclude-->

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "_call_out_test_projects.md"

## A Note To Developers

Once Testcontainers creates a server instance, developers may use the connection string with any of the popular data-access technologies found in the .NET ecosystem. Some of these libraries include [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore), [Dapper](https://www.nuget.org/packages/Dapper), and [NHibernate](https://www.nuget.org/packages/NHibernate). At which point, developers can execute database migrations and SQL scripts.