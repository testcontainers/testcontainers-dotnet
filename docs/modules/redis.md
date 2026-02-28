# Redis

[Redis](https://redis.io/) is an in-memory key–value database, used as a distributed cache and message broker, with optional durability.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Redis
```

You can start a Redis container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Redis.Tests/RedisContainerTest.cs:UseRedisContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Redis.Tests/Testcontainers.Redis.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"