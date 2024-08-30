# MongoDB

[MongoDB](https://www.mongodb.com/what-is-mongodb) is a cross-platform document-oriented database. MongoDB's document model is simple for developers to use within their applications, while still providing all the complex capabilities of traditional relational databases.

Add the following dependency to your project file:

```console title="NuGet"
dotnet add package Testcontainers.MongoDb
```

You can start an MongoDB container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

<!--codeinclude-->
[Create Container Instance](../../tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs) inside_block:CreateMongoDbContainer
<!--/codeinclude-->

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

<!--codeinclude-->
[Usage Example](../../tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs) inside_block:UseMongoDbContainer
<!--/codeinclude-->

The test example uses the following NuGet dependencies:

<!--codeinclude-->
[Package References](../../tests/Testcontainers.MongoDb.Tests/Testcontainers.MongoDb.Tests.csproj) inside_block:PackageReferences
<!--/codeinclude-->

To execute the tests, use the command `dotnet test` from a terminal.

## MongoDb Replica Set

By default, MongoDB runs as a standalone instance. If your tests require a MongoDB replica set, use the following configuration which will initialize a single-node replica set:

<!--codeinclude-->
[Usage Example](../../tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs) inside_block:ReplicaSetContainerConfiguration
<!--/codeinclude-->