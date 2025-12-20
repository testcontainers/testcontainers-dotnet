# PostgreSQL

[PostgreSQL](https://www.postgresql.org/) is a powerful, open-source relational database management system (RDBMS) used to store, manage, and retrieve structured data.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.PostgreSql
```

You can start a PostgreSQL container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a PostgreSQL container"
    ```csharp
    var postgreSqlContainer = new PostgreSqlBuilder("postgres:15.1").Build();
    await postgreSqlContainer.StartAsync();
    ```

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs:UsePostgreSqlContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.PostgreSql.Tests/Testcontainers.PostgreSql.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"