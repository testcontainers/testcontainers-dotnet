# Neo4j

[Neo4j](https://neo4j.com/product/neo4j-graph-database/) is a graph database designed to work with nodes and edges. It is an ACID-compliant transactional graph database engine, and developers can communicate with it using the HTTP endpoint or by using the **Bolt** protocol.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Neo4j
```

You can start an Neo4j container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Neo4j.Tests/Neo4jContainerTest.cs:CreateNeo4jContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Neo4j.Tests/Neo4jContainerTest.cs:UseNeo4jContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Neo4j.Tests/Testcontainers.Neo4j.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"