# Chroma

[Chroma](https://www.trychroma.com/) is an open-source vector database for AI applications. It stores embeddings with their documents and metadata, and finds the nearest ones to a query, through a REST API.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Chroma
```

You can start a Chroma container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Chroma.Tests/ChromaDefaultContainerTest.cs:UseChromaContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Chroma.Tests/Testcontainers.Chroma.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Chroma versions

The container is ready when the heartbeat of the Chroma API answers. Chroma 0.5.16 and later answer the heartbeat of the v2 API, and the earlier releases have only the v1 API, so the module asks both: it works with any Chroma image, whatever its tag.
