# MongoDB Atlas Local

[MongoDB Atlas Local](https://www.mongodb.com/docs/atlas/cli/current/atlas-cli-deploy-docker/) runs a local Atlas deployment in a single container. Next to MongoDB, it includes the Atlas Search process, so features like [Atlas Search](https://www.mongodb.com/docs/atlas/atlas-search/) (`$search`) and [Atlas Vector Search](https://www.mongodb.com/docs/atlas/atlas-vector-search/vector-search-overview/) (`$vectorSearch`) can be tested without an Atlas cluster.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.MongoDbAtlasLocal
```

You can start a MongoDB Atlas Local container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations. Authentication is disabled by default. Set a username and a password to enable it.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/MongoDbAtlasLocalContainerTest.cs:CreateMongoDbAtlasLocalContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/MongoDbAtlasLocalContainerTest.cs:UseMongoDbAtlasLocalContainer"
    ```

=== "Atlas Search Helper"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/AtlasSearch.cs"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/Testcontainers.MongoDbAtlasLocal.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

!!! note

    Atlas Search builds indexes asynchronously. A search index becomes queryable shortly after it has been created, and newly written documents become searchable shortly after they have been written. Wait until the index reports `queryable: true` before running search queries, and retry a query until it returns the documents you expect, as the Atlas Search helper above does.

## Seeding the deployment

Init scripts seed the deployment on its first start. `WithInitScript(string)` copies a script file from the test host, and `WithInitScriptContent(string, string)` creates one from a string. JavaScript (`.js`) scripts run in `mongosh` against the database set with `WithInitDatabase(string)` (default `test`), and shell (`.sh`) scripts run in `bash`. The container silently skips files with any other extension, so the builder rejects them. Scripts run in alphabetical order of their file names and finish before the container is reported ready. They can also create Atlas Search indexes. A restarted or reused container keeps its data and does not run the scripts again.

=== "Seed Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/MongoDbAtlasLocalSeedTest.cs:SeedMongoDbAtlasLocalContainer"
    ```

=== "Seed Script"
    ```javascript
    --8<-- "tests/Testcontainers.MongoDbAtlasLocal.Tests/Seed/01-movies.js"
    ```

## Telemetry

The MongoDB Atlas Local image sends telemetry to MongoDB by default. Call `WithNoTelemetry()` to disable it, as shown in the seed configuration above.
