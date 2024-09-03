# MongoDB

[MongoDB](https://www.mongodb.com/what-is-mongodb) is a cross-platform document-oriented database. MongoDB's document model is simple for developers to use within their applications, while still providing all the complex capabilities of traditional relational databases.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.MongoDb
```

You can start a MongoDB container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs:CreateMongoDbContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs:UseMongoDbContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.MongoDb.Tests/Testcontainers.MongoDb.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## MongoDb Replica Set

By default, MongoDB runs as a standalone instance. If your tests require a MongoDB replica set, use the following configuration which will initialize a single-node replica set:

=== "Replica Set Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs:ReplicaSetContainerConfiguration"
    ```