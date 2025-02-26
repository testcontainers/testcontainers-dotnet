# Cassandra

[Cassandra](https://cassandra.apache.org/) is a powerful, open-source NoSQL distributed, highly available, fault-tolerant database used to store, manage, and retrieve structured data.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Cassandra
```

You can start a Cassandra container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Cassandra.Tests/CassandraContainerTest.cs:UseCassandraContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Cassandra.Tests/Testcontainers.Cassandra.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"
