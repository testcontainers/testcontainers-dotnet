# MongoDB Atlas Local

[MongoDB Atlas Local](https://www.mongodb.com/docs/atlas/cli/current/atlas-cli-deploy-docker/) runs a local Atlas deployment in a single container. In addition to MongoDB, it runs the Atlas Search process, so you can test [Atlas Search](https://www.mongodb.com/docs/atlas/atlas-search/) and [Atlas Vector Search](https://www.mongodb.com/docs/atlas/atlas-vector-search/vector-search-overview/) without an Atlas cluster.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.MongoDbAtlasLocal
```

You can start a MongoDB Atlas Local container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/MongoDbAtlasLocalContainerTest.cs:CreateMongoDbAtlasLocalContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/MongoDbAtlasLocalContainerTest.cs:UseMongoDbAtlasLocalContainer"
    ```

The test example creates an Atlas Search index. Use `GetConnectionString()` to configure the MongoDB client. The default configuration creates the root user `mongo` with the password `mongo`. Use `WithUsername(string)` and `WithPassword(string)` to configure different credentials, or set both to an empty string to disable authentication.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/Testcontainers.MongoDbAtlasLocal.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Init scripts

Use `WithInitScript(string)` or `WithInitScriptContent(string, string)` to seed the deployment on its first start. JavaScript (`.js`) scripts run in `mongosh` against the database set with `WithInitDatabase(string)`, which defaults to `test`. Shell (`.sh`) scripts run in `bash`. The scripts run in alphabetical order of their file names before the container is ready:

=== "Init Script Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/MongoDbAtlasLocalContainerTest.cs:InitScriptContainerConfiguration"
    ```

A restarted or reused container keeps its data and does not run the init scripts again.