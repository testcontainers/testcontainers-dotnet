# Chroma

[Chroma](https://www.trychroma.com/) is an open-source vector database for AI applications. It stores embeddings together with their documents and metadata, and runs nearest neighbor queries against them.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Chroma
```

You can start a Chroma container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Chroma.Tests/ChromaContainerTest.cs:CreateChromaContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Chroma.Tests/ChromaContainerTest.cs:UseChromaContainer"
    ```

The test example creates a collection, adds two records, and queries the record nearest to an embedding. Use `GetBaseAddress()` to configure the Chroma client.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Chroma.Tests/Testcontainers.Chroma.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"
