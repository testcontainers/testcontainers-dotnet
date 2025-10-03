# Valkey

[Valkey](https://valkey.io/) is an open-source, high-performance data structure server that serves as a drop-in replacement for Redis. It supports various data structures such as strings, hashes, lists, sets, sorted sets with range queries, bitmaps, hyperloglogs, geospatial indexes, and streams.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Valkey
```

You can start a Valkey container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Valkey.Tests/ValkeyContainerTest.cs:CreateValkeyContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Valkey.Tests/ValkeyContainerTest.cs:UseValkeyContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Valkey.Tests/Testcontainers.Valkey.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Connection String

The Valkey module provides a `GetConnectionString()` method that returns a connection string compatible with StackExchange.Redis and other Redis client libraries that support Valkey:

```csharp
var connectionString = _valkeyContainer.GetConnectionString();
using var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
```

## Executing Scripts

You can execute Lua scripts against the Valkey container using the standard Redis client libraries:

```csharp
const string scriptContent = "return 'Hello, Valkey!'";
var execResult = await _valkeyContainer.ExecScriptAsync(scriptContent);
```