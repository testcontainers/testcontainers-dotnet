# PostgreSQL

[PostgreSQL](https://www.postgresql.org/) is a powerful, open-source relational database management system (RDBMS) used to store, manage, and retrieve structured data.

Add the following dependency to your project file:

```console title="NuGet"
dotnet add package Testcontainers.PostgreSql
```

You can start an PostgreSQL container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

<!--codeinclude-->
[Usage Example](../../tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs) inside_block:UsePostgreSqlContainer
<!--/codeinclude-->

The test example uses the following NuGet dependencies:

<!--codeinclude-->
[Package References](../../tests/Testcontainers.PostgreSql.Tests/Testcontainers.PostgreSql.Tests.csproj) inside_block:PackageReferences
<!--/codeinclude-->

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.md"