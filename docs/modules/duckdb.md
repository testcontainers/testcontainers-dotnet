# DuckDB

[DuckDB](https://duckdb.org/) is a fast, in-process analytical database. It runs embedded within a host process and stores data in a single database file.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.DuckDb
```

You can start a DuckDB container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Test class"
    ```csharp
    --8<-- "tests/Testcontainers.DuckDb.Tests/DuckDbContainerTest.docs.cs:UseDuckDbContainer"
    }
    ```

Execute a SQL script:

=== "Run SQL script"
    ```csharp
    --8<-- "tests/Testcontainers.DuckDb.Tests/DuckDbContainerTest.docs.cs:RunSQLScript"
    ```

!!! note

    DuckDB is an embedded database — the [duckdb/duckdb](https://hub.docker.com/r/duckdb/duckdb) image ships the DuckDB CLI, not a database server. The container keeps an in-memory DuckDB CLI process running to stay alive, and `ExecScriptAsync(string)` runs SQL scripts against the configured database file (default: `/database.duckdb`, created on first use). The database state persists across script executions. Use `WithDatabase(string)` to configure a different database file path, and `GetDatabaseFilePath()` together with `ReadFileAsync(string)` to copy the database file to the test host.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.DuckDb.Tests/Testcontainers.DuckDb.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"
